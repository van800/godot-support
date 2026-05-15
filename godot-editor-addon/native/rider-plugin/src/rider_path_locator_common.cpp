// Common, platform-agnostic pieces of RiderPathLocator reworked for GDExtension

#include "rider_path_locator.h"

#include <algorithm>
#include <climits>
#include <sstream>

#include <godot_cpp/classes/dir_access.hpp>
#include <godot_cpp/classes/file_access.hpp>
#include <godot_cpp/classes/reg_ex.hpp>
#include <godot_cpp/classes/reg_ex_match.hpp>
#include <godot_cpp/variant/string.hpp>

using namespace godot;

// -- conversion helpers -------------------------------------------------------

static String gstr(const std::string &s) { return String(s.c_str()); }
static std::string stdstr(const String &s) { return std::string(s.utf8().get_data()); }

// --- Version -----------------------------------------------------------------

static int to_int(const std::string &s) {
	if (s.empty())
		return -1;
	char *end = nullptr;
	errno = 0;
	long v = std::strtol(s.c_str(), &end, 10);
	if (end == s.c_str() || *end != '\0')
		return -1;
	if (errno == ERANGE || v < INT_MIN || v > INT_MAX)
		return -1;
	return static_cast<int>(v);
}

void Version::operator=(const std::string &version_str) {
	parts.clear();
	std::stringstream ss(version_str);
	std::string item;
	while (std::getline(ss, item, '.')) {
		parts.push_back(to_int(item));
	}
}

bool Version::operator<(const Version &rhs) const {
	size_t l = parts.size();
	size_t r = rhs.parts.size();
	size_t n = std::min(l, r);
	for (size_t i = 0; i < n; ++i) {
		if (parts[i] < rhs.parts[i])
			return true;
		if (parts[i] > rhs.parts[i])
			return false;
	}
	return l < r;
}

// --- Helpers -----------------------------------------------------------------

bool RiderPathLocator::directory_exists_and_non_empty(const std::string &path) {
	String gpath = gstr(path);
	if (!DirAccess::dir_exists_absolute(gpath))
		return false;
	return DirAccess::get_files_at(gpath).size() > 0 || DirAccess::get_directories_at(gpath).size() > 0;
}

// Extract install_location from toolbox .settings.json
std::string RiderPathLocator::extract_install_location_from_settings_json(const std::string &toolbox_path) {
	String settings = gstr(toolbox_path).path_join(".settings.json");
	String json = FileAccess::get_file_as_string(settings);
	if (json.is_empty())
		return {};
	auto re = RegEx::create_from_string("\"install_location\"\\s*:\\s*\"([^\"]*)\"");
	if (!re.is_valid())
		return {};
	auto m = re->search(json);
	if (m.is_valid() && m->get_group_count() >= 1)
		return stdstr(m->get_string(1));
	return {};
}

// Find nearest .history.json upwards
std::string RiderPathLocator::get_history_json_path(const std::string &rider_path) {
	String dir = gstr(rider_path).get_base_dir();
	for (int i = 0; i < 10; ++i) {
		String candidate = dir.path_join(".history.json");
		if (FileAccess::file_exists(candidate))
			return stdstr(candidate);
		String parent = dir.get_base_dir();
		if (parent == dir)
			break;
		dir = parent;
	}
	return {};
}

Version RiderPathLocator::get_last_build_version(const std::string &history_json_path) {
	if (history_json_path.empty())
		return {};
	String json = FileAccess::get_file_as_string(gstr(history_json_path));
	if (json.is_empty())
		return {};
	auto re = RegEx::create_from_string("\"build\"\\s*:\\s*\"([0-9.]+)\"");
	if (!re.is_valid())
		return {};
	// keep last match
	Version v;
	auto matches = re->search_all(json);
	for (int i = 0; i < matches.size(); ++i) {
		Ref<RegExMatch> m = matches[i];
		if (m.is_valid() && m->get_group_count() >= 1)
			v = Version(stdstr(m->get_string(1)));
	}
	return v;
}

void RiderPathLocator::parse_product_info_json(InstallInfo &info, const std::string &product_info_json_path) {
	String json = FileAccess::get_file_as_string(gstr(product_info_json_path));
	if (json.is_empty())
		return;

	auto build_re = RegEx::create_from_string("\"buildNumber\"\\s*:\\s*\"([0-9.]+)\"");
	if (build_re.is_valid()) {
		auto m = build_re->search(json);
		if (m.is_valid() && m->get_group_count() >= 1) {
			info.version = Version(stdstr(m->get_string(1)));
			if (info.version.major() >= 221) {
				info.support = InstallInfo::SupportUproject::Release;
				return;
			}
		}
	}

	// Fallback: look for SupportUproject or SupportUprojectState in customProperties
	auto state_re = RegEx::create_from_string(
			"\"(SupportUproject|SupportUprojectState)\".*?\"value\"\\s*:\\n?\\t?\\s*\"(Beta|Release)\"");
	if (state_re.is_valid()) {
		auto m = state_re->search(json);
		if (m.is_valid() && m->get_group_count() >= 2) {
			String val = m->get_string(2);
			if (val == "Beta")
				info.support = InstallInfo::SupportUproject::Beta;
			if (val == "Release")
				info.support = InstallInfo::SupportUproject::Release;
		}
	}
}

std::vector<InstallInfo> RiderPathLocator::get_install_infos(const std::string &toolbox_rider_root_path,
		const std::string &pattern,
		InstallInfo::InstallType type) {
	std::vector<InstallInfo> result;
	if (!directory_exists_and_non_empty(toolbox_rider_root_path))
		return result;

	// Convert glob pattern to anchored regex: '.' → '[.]', '*' → '.*'
	String gpattern = String("^") + gstr(pattern).replace(".", "[.]").replace("*", ".*") + String("$");
	auto rx = RegEx::create_from_string(gpattern);
	if (!rx.is_valid())
		return result;

	// Iterative recursive traversal
	std::vector<String> dirs_to_visit = { gstr(toolbox_rider_root_path) };
	while (!dirs_to_visit.empty()) {
		String current = dirs_to_visit.back();
		dirs_to_visit.pop_back();

		PackedStringArray files = DirAccess::get_files_at(current);
		for (int i = 0; i < files.size(); ++i) {
			if (!rx->search(files[i]).is_valid())
				continue;
			String full_path = current.path_join(files[i]);
			auto info = get_install_info_from_rider_path(stdstr(full_path), type);
			if (!info.has_value())
				continue;
			Version last = get_last_build_version(get_history_json_path(stdstr(full_path)));
			if (last.initialized() && !(info->version == last))
				continue;
			result.push_back(*info);
		}

		PackedStringArray subdirs = DirAccess::get_directories_at(current);
		for (int i = 0; i < subdirs.size(); ++i) {
			dirs_to_visit.push_back(current.path_join(subdirs[i]));
		}
	}
	return result;
}

std::vector<InstallInfo> RiderPathLocator::get_install_infos_from_toolbox(const std::string &toolbox_path,
		const std::string &pattern) {
	if (!directory_exists_and_non_empty(toolbox_path))
		return {};

	const std::string install_location = extract_install_location_from_settings_json(toolbox_path);
	if (!install_location.empty()) {
		// V1 custom location
		auto r = get_install_infos(stdstr(gstr(install_location).path_join("apps")), pattern, InstallInfo::InstallType::Toolbox);
		if (!r.empty())
			return r;
		// V2 custom location
		return get_install_infos(install_location, pattern, InstallInfo::InstallType::Toolbox);
	}

	// V1 default location
	auto r = get_install_infos(stdstr(gstr(toolbox_path).path_join("apps")), pattern, InstallInfo::InstallType::Toolbox);
	if (!r.empty())
		return r;

	// V2 default location
	return get_install_infos(get_default_ide_install_location_for_toolbox_v2(), pattern, InstallInfo::InstallType::Toolbox);
}

// Resource file not available in Godot context; keep as empty but leave hook for future.
std::vector<InstallInfo> RiderPathLocator::get_install_infos_from_resource_file() {
	return {};
}

bool InstallInfoLess::operator()(const InstallInfo &a, const InstallInfo &b) const {
	if (a.version < b.version)
		return true;
	if (b.version < a.version)
		return false;
	// normalize path separators
	auto norm = [](std::string s) {
		std::replace(s.begin(), s.end(), '\\', '/');
		return s;
	};
	return norm(a.path) < norm(b.path);
}