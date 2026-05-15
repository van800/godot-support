#pragma once

#include <string>
#include <vector>
#include <set>
#include <optional>

struct Version {
    std::vector<int> parts;

    Version() = default;
    explicit Version(const std::string &version_str) { *this = version_str; }

    void operator=(const std::string &version_str);

    int major() const { return parts.size() >= 1 ? parts[0] : -1; }
    int minor() const { return parts.size() >= 2 ? parts[1] : -1; }
    int patch() const { return parts.size() >= 3 ? parts[2] : -1; }

    bool initialized() const { return !parts.empty(); }

    bool operator<(const Version &rhs) const;
    bool operator==(const Version &rhs) const { return !(*this < rhs) && !(rhs < *this); }
};

struct InstallInfo {
    enum class SupportUproject { None, Beta, Release };
    enum class InstallType { Installed, Toolbox, Custom };

    std::string path;               // Path to rider executable or application root
    Version version;                // Build version
    SupportUproject support = SupportUproject::None;
    InstallType type = InstallType::Installed;
};

struct InstallInfoLess {
    bool operator()(const InstallInfo &a, const InstallInfo &b) const;
};

class RiderPathLocator {
public:
    // Returns InstallInfo if the path looks like a Rider installation with rider-cpp plugin.
    static std::optional<InstallInfo> get_install_info_from_rider_path(const std::string &path,
                                                                       InstallInfo::InstallType type);

    static bool directory_exists_and_non_empty(const std::string &path);
    static std::string extract_install_location_from_settings_json(const std::string &toolbox_path);

    // Collect Rider installs from multiple locations (Toolbox, manual, system search etc.).
    static std::set<InstallInfo, InstallInfoLess> collect_all_paths();

private:
    static void parse_product_info_json(InstallInfo &info, const std::string &product_info_json_path);
    static std::string get_default_ide_install_location_for_toolbox_v2();
    static std::vector<InstallInfo> get_install_infos_from_toolbox(const std::string &toolbox_path,
                                                                   const std::string &pattern);
#if __APPLE__
	static std::vector<InstallInfo> get_install_infos_from_toolbox_mac(const std::string &toolbox_path,
																   const std::string &pattern);
	static std::vector<InstallInfo> get_install_infos_mac(const std::string &toolbox_rider_root_path,
												  const std::string &pattern,
												  InstallInfo::InstallType type);
#endif
	
	static std::vector<InstallInfo> get_install_infos_from_resource_file();
    static std::vector<InstallInfo> get_install_infos(const std::string &toolbox_rider_root_path,
                                                      const std::string &pattern,
                                                      InstallInfo::InstallType type);

    static std::string get_history_json_path(const std::string &rider_path);
    static Version get_last_build_version(const std::string &history_json_path);
};
