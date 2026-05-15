// Linux-specific parts of RiderPathLocator for GDExtension builds

#include "rider_path_locator.h"

#if defined(__linux__)

#include <godot_cpp/classes/dir_access.hpp>
#include <godot_cpp/classes/file_access.hpp>
#include <godot_cpp/classes/os.hpp>
#include <godot_cpp/core/print_string.hpp>
#include <godot_cpp/variant/string.hpp>

using namespace godot;

// -- conversion helpers -------------------------------------------------------

static String gstr(const std::string &s) { return String(s.c_str()); }
static std::string stdstr(const String &s) { return std::string(s.utf8().get_data()); }

// -- RiderPathLocator ---------------------------------------------------------

std::string RiderPathLocator::get_default_ide_install_location_for_toolbox_v2() {
	// Same as V1 on Linux
	return {};
}

std::optional<InstallInfo> RiderPathLocator::get_install_info_from_rider_path(
		const std::string &path, InstallInfo::InstallType type) {
	String gpath = gstr(path);
	if (!FileAccess::file_exists(gpath))
		return std::nullopt;

	// Expect .../bin/rider.sh — derive installation root by going two levels up
	const String bin_dir = gpath.get_base_dir();
	if (bin_dir.get_file() != "bin")
		return std::nullopt;
	const String rider_dir = bin_dir.get_base_dir();

	if (!directory_exists_and_non_empty(stdstr(rider_dir.path_join("plugins"))))
		return std::nullopt;

	InstallInfo info;
	info.path = path;
	info.type = type;
	String product_info = rider_dir.path_join("product-info.json");
	if (FileAccess::file_exists(product_info)) {
		parse_product_info_json(info, stdstr(product_info));
	}
	if (!info.version.initialized()) {
		// Try to parse folder name as version
		info.version = stdstr(rider_dir.get_file());
		if (info.version.major() >= 221)
			info.support = InstallInfo::SupportUproject::Release;
	}
	return info;
}

// -- static helpers -----------------------------------------------------------

static String get_home() {
	return OS::get_singleton()->get_environment("HOME");
}

static std::vector<InstallInfo> get_manually_installed_riders() {
	std::vector<InstallInfo> result;

	std::vector lookup_paths = {
		get_home(),
		String("/opt"),
		String("/usr/local/bin"),
	};

	for (const auto &base : lookup_paths) {
		if (!DirAccess::dir_exists_absolute(base))
			continue;
		PackedStringArray dirs = DirAccess::get_directories_at(base);
		for (int i = 0; i < dirs.size(); ++i) {
			String dir_name = dirs[i];
			if (!dir_name.match("*Rider*"))
				continue;
			String full = base.path_join(dir_name).path_join("bin").path_join("rider.sh");
			try {
				auto info = RiderPathLocator::get_install_info_from_rider_path(
						stdstr(full), InstallInfo::InstallType::Installed);
				if (info.has_value())
					result.push_back(*info);
			} catch (const std::exception &e) {
				print_error(
						String("godot-engine.rider-plugin failed to process ") + full + String(": ") + String(e.what()));
			}
		}
	}

	// Snap
	String snap_path("/snap/rider/current/bin/rider.sh");
	auto snap_info = RiderPathLocator::get_install_info_from_rider_path(
			stdstr(snap_path), InstallInfo::InstallType::Installed);
	if (snap_info.has_value())
		result.push_back(*snap_info);

	return result;
}

static String get_toolbox_path() {
	return get_home().path_join(".local").path_join("share").path_join("JetBrains").path_join("Toolbox");
}

static std::vector<InstallInfo> get_installed_riders_with_locate() {
	std::vector<InstallInfo> result;

	PackedStringArray args;
	args.push_back("-e");
	args.push_back("bin/rider.sh");
	Array output;
	int exit_code = OS::get_singleton()->execute("locate", args, output);
	if (exit_code < 0 || output.is_empty())
		return result;

	PackedStringArray lines = String(output[0]).split("\n");
	for (int i = 0; i < lines.size(); ++i) {
		String line = lines[i].strip_edges();
		if (line.is_empty())
			continue;
		if (line.contains("snapd") || line.contains(".local") || line.contains("/opt"))
			continue;
		try {
			auto info = RiderPathLocator::get_install_info_from_rider_path(
					stdstr(line), InstallInfo::InstallType::Installed);
			if (info.has_value())
				result.push_back(*info);
		} catch (const std::exception &e) {
			print_error(String("godot-engine.rider-plugin [locate] failed on ") + line + String(": ") + String(e.what()));
		}
	}
	return result;
}

std::set<InstallInfo, InstallInfoLess> RiderPathLocator::collect_all_paths() {
	std::set<InstallInfo, InstallInfoLess> s;
	try {
		for (auto &i : get_installed_riders_with_locate())
			s.insert(i);
		for (auto &i : get_manually_installed_riders())
			s.insert(i);
		for (auto &i : get_install_infos_from_toolbox(stdstr(get_toolbox_path()), "[Rr]ider.sh"))
			s.insert(i);
		for (auto &i : get_install_infos_from_resource_file())
			s.insert(i);
	} catch (const std::exception &e) {
		print_error(String("godot-engine.rider-plugin: ") + String(e.what()));
	}
	return s;
}

#endif // __linux__