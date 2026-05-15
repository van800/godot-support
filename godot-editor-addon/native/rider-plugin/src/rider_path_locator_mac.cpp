// macOS-specific parts of RiderPathLocator for GDExtension builds

#include "rider_path_locator.h"

#if __APPLE__

#include <filesystem>
#include <regex>
#include <string>
#include <vector>
#include <cstdlib>
#include <cstdio>
// Needed for std::stringstream used with std::getline below
#include <sstream>
#include <fstream>

namespace fs = std::filesystem;

std::optional<InstallInfo> RiderPathLocator::get_install_info_from_rider_path(const std::string &path_to_rider_app,
                                                                              InstallInfo::InstallType type) {
    // Expecting path to Rider.app
    if (!directory_exists_and_non_empty(path_to_rider_app)) return std::nullopt;

    const fs::path app(path_to_rider_app);
    const fs::path plugins = app / "Contents" / "plugins";
    if (!directory_exists_and_non_empty(plugins.string())) return std::nullopt;

    InstallInfo info;
    info.path = (app / "Contents" / "MacOS" / "rider").string();
    info.type = type;
    const fs::path product_info = app / "Contents" / "Resources" / "product-info.json";
    if (fs::exists(product_info)) {
        parse_product_info_json(info, product_info.string());
    }
    return info;
}

static std::vector<InstallInfo> get_manually_installed_riders() {
    std::vector<InstallInfo> result;
    const fs::path apps("/Applications");
    if (!fs::exists(apps)) return result;
    std::regex rx("Rider.*\\.app");
    for (auto const &p : fs::directory_iterator(apps)) {
        if (!p.is_directory()) continue;
        const std::string name = p.path().filename().string();
        if (!std::regex_match(name, rx)) continue;
        auto info = RiderPathLocator::get_install_info_from_rider_path(p.path().string(), InstallInfo::InstallType::Installed);
        if (info.has_value()) result.push_back(*info);
    }
    return result;
}

static std::string get_toolbox_path() {
    const char *home = std::getenv("HOME");
    std::string base = home ? home : "";
    return (fs::path(base) / "Library" / "Application Support" / "JetBrains" / "Toolbox").string();
}

std::string RiderPathLocator::get_default_ide_install_location_for_toolbox_v2() {
    const char *home = std::getenv("HOME");
    std::string base = home ? home : "";
    return (fs::path(base) / "Applications").string();
}

// Use `mdfind` to locate Rider apps (best-effort, optional)
static std::vector<InstallInfo> get_installed_riders_with_mdfind() {
    std::vector<InstallInfo> result;
    FILE *pipe = popen("/usr/bin/mdfind \"kMDItemKind == Application\"", "r");
    if (!pipe) return result;
    char buffer[4096];
    std::string out;
    while (fgets(buffer, sizeof(buffer), pipe)) out += buffer;
    pclose(pipe);
    std::stringstream ss(out);
    std::string line;
    while (std::getline(ss, line)) {
        if (line.find("Rider") == std::string::npos) continue;
        auto info = RiderPathLocator::get_install_info_from_rider_path(line, InstallInfo::InstallType::Installed);
        if (info.has_value()) result.push_back(*info);
    }
    return result;
}

std::vector<InstallInfo> RiderPathLocator::get_install_infos_mac(const std::string &toolbox_rider_root_path,
                                                             const std::string &pattern,
                                                             InstallInfo::InstallType type) {
    std::vector<InstallInfo> result;
    if (!directory_exists_and_non_empty(toolbox_rider_root_path)) return result;

    std::regex rx;
    // Convert very small subset of glob to regex: '*' -> ".*"
    std::string rx_str = std::regex_replace(pattern, std::regex("[.]"), "[.]");
    rx_str = std::regex_replace(rx_str, std::regex("\\*"), ".*");
    rx = std::regex(rx_str);

    for (auto it = fs::recursive_directory_iterator(toolbox_rider_root_path); it != fs::end(it); ++it) {
        const auto &p = *it;
        if (!p.is_directory()) continue;
        const std::string filename = p.path().filename().string();
        if (!std::regex_match(filename, rx)) continue;

        auto info = get_install_info_from_rider_path(p.path().string(), type);
        if (!info.has_value()) continue;
        Version last = get_last_build_version(get_history_json_path(p.path().string()));
        if (last.initialized() && !(info->version == last)) continue;
        result.push_back(*info);
        it.disable_recursion_pending();
    }
    return result;
}

std::vector<InstallInfo> RiderPathLocator::get_install_infos_from_toolbox_mac(const std::string &toolbox_path,
                                                                          const std::string &pattern) {
    if (!directory_exists_and_non_empty(toolbox_path)) return {};

    const std::string install_location = extract_install_location_from_settings_json(toolbox_path);
    if (!install_location.empty()) {
        // V1 custom location
        auto r = get_install_infos_mac((fs::path(install_location) / "apps").string(), pattern, InstallInfo::InstallType::Toolbox);
        if (!r.empty()) return r;
        // V2 custom location
        return get_install_infos_mac(install_location, pattern, InstallInfo::InstallType::Toolbox);
    }

    // V1 default location
    auto r = get_install_infos_mac((fs::path(toolbox_path) / "apps").string(), pattern, InstallInfo::InstallType::Toolbox);
    if (!r.empty()) return r;

    // V2 default location
    return get_install_infos_mac(get_default_ide_install_location_for_toolbox_v2(), pattern, InstallInfo::InstallType::Toolbox);
}

std::set<InstallInfo, InstallInfoLess> RiderPathLocator::collect_all_paths() {
    std::set<InstallInfo, InstallInfoLess> s;
    for (auto &i : get_installed_riders_with_mdfind()) s.insert(i);
    for (auto &i : get_manually_installed_riders()) s.insert(i);
    for (auto &i : get_install_infos_from_toolbox_mac(get_toolbox_path(), "Rider*.app")) s.insert(i);
    for (auto &i : get_install_infos_from_resource_file()) s.insert(i);
    return s;
}

#endif // __APPLE__
