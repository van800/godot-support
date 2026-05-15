// Windows-specific parts of RiderPathLocator for GDExtension builds

#include "rider_path_locator.h"

#if defined(_WIN32)

#include <windows.h>
#include <winreg.h>

#include <algorithm>
#include <filesystem>
#include <optional>
#include <regex>
#include <string>
#include <vector>
#include <cstdlib>

namespace fs = std::filesystem;

// --- Helpers -----------------------------------------------------------------

static std::string w2u8(const std::wstring &ws) {
    if (ws.empty()) return {};
    int size_needed = WideCharToMultiByte(CP_UTF8, 0, ws.c_str(), (int)ws.size(), nullptr, 0, nullptr, nullptr);
    if (size_needed <= 0) return {};
    std::string s(size_needed, '\0');
    WideCharToMultiByte(CP_UTF8, 0, ws.c_str(), (int)ws.size(), s.data(), size_needed, nullptr, nullptr);
    return s;
}

static std::wstring u82w(const std::string &s) {
    if (s.empty()) return {};
    int size_needed = MultiByteToWideChar(CP_UTF8, 0, s.c_str(), (int)s.size(), nullptr, 0);
    if (size_needed <= 0) return {};
    std::wstring ws(size_needed, L'\0');
    MultiByteToWideChar(CP_UTF8, 0, s.c_str(), (int)s.size(), ws.data(), size_needed);
    return ws;
}

static std::string get_env_localappdata() {
    const char *p = std::getenv("LOCALAPPDATA");
    return p ? std::string(p) : std::string();
}

// Query default value of the key as UTF-16 string
static LONG reg_query_default_string(HKEY key, std::wstring &out) {
    DWORD type = 0;
    DWORD size = 0;
    LONG r = RegQueryValueExW(key, nullptr, nullptr, &type, nullptr, &size);
    if (r != ERROR_SUCCESS || (type != REG_SZ && type != REG_EXPAND_SZ)) return r;
    std::wstring buf;
    buf.resize(size / sizeof(wchar_t));
    r = RegQueryValueExW(key, nullptr, nullptr, &type, reinterpret_cast<LPBYTE>(buf.data()), &size);
    if (r == ERROR_SUCCESS) {
        // Ensure null-terminated, strip trailing nulls
        while (!buf.empty() && buf.back() == L'\0') buf.pop_back();
        out = buf;
    }
    return r;
}

static LONG reg_query_string_value(HKEY key, const wchar_t *name, std::wstring &out) {
    DWORD type = 0;
    DWORD size = 0;
    LONG r = RegQueryValueExW(key, name, nullptr, &type, nullptr, &size);
    if (r != ERROR_SUCCESS || (type != REG_SZ && type != REG_EXPAND_SZ)) return r;
    std::wstring buf;
    buf.resize(size / sizeof(wchar_t));
    r = RegQueryValueExW(key, name, nullptr, &type, reinterpret_cast<LPBYTE>(buf.data()), &size);
    if (r == ERROR_SUCCESS) {
        while (!buf.empty() && buf.back() == L'\0') buf.pop_back();
        out = buf;
    }
    return r;
}

static std::vector<std::wstring> enumerate_subkeys(HKEY key) {
    std::vector<std::wstring> names;
    for (DWORD index = 0;; ++index) {
        wchar_t name[256];
        DWORD name_len = static_cast<DWORD>(std::size(name));
        FILETIME dummy{};
        LONG res = RegEnumKeyExW(key, index, name, &name_len, nullptr, nullptr, nullptr, &dummy);
        if (res == ERROR_NO_MORE_ITEMS) break;
        if (res != ERROR_SUCCESS) { names.clear(); break; }
        names.emplace_back(name);
    }
    return names;
}

// --- Platform-specific methods ----------------------------------------------

std::string RiderPathLocator::get_default_ide_install_location_for_toolbox_v2() {
    // As per JetBrains Toolbox on Windows: %LOCALAPPDATA%\Programs
    fs::path p = fs::path(get_env_localappdata()) / "Programs";
    return p.string();
}

std::optional<InstallInfo> RiderPathLocator::get_install_info_from_rider_path(const std::string &path,
                                                                              InstallInfo::InstallType type) {
    if (!fs::exists(path)) return std::nullopt;

    // Expect ...\\bin\\rider64.exe
    std::regex re("(.*)(?:\\\\|/)bin");
    std::smatch m;
    if (!std::regex_search(path, m, re) || m.size() < 2) return std::nullopt;
    const fs::path rider_dir = m[1].str();

    const fs::path plugins = rider_dir / "plugins";
    if (!directory_exists_and_non_empty(plugins.string())) return std::nullopt;

    InstallInfo info;
    info.path = path;
    info.type = type;
    const fs::path product_info = rider_dir / "product-info.json";
    if (fs::exists(product_info)) parse_product_info_json(info, product_info.string());
    if (!info.version.initialized()) {
        info.version = rider_dir.filename().string();
        if (info.version.major() >= 221) info.support = InstallInfo::SupportUproject::Release;
    }
    return info;
}

// Registry scanning for installed Rider locations (Uninstall keys)
static std::vector<InstallInfo> collect_paths_from_uninstall_registry(HKEY root, const wchar_t *reg_key) {
    std::vector<InstallInfo> result;
    HKEY key = nullptr;
    if (RegOpenKeyExW(root, reg_key, 0, KEY_READ, &key) != ERROR_SUCCESS) return result;

    auto subkeys = enumerate_subkeys(key);
    for (const auto &name : subkeys) {
        if (name.find(L"Rider") == std::wstring::npos) continue;
        HKEY sub = nullptr;
        if (RegOpenKeyExW(key, name.c_str(), 0, KEY_READ, &sub) != ERROR_SUCCESS) continue;
        std::wstring install_dir_w;
        if (reg_query_string_value(sub, L"InstallLocation", install_dir_w) == ERROR_SUCCESS) {
            fs::path exe = fs::path(w2u8(install_dir_w)) / "bin" / "rider64.exe";
            auto info = RiderPathLocator::get_install_info_from_rider_path(exe.string(), InstallInfo::InstallType::Installed);
            if (info.has_value()) result.push_back(*info);
        }
        RegCloseKey(sub);
    }
    RegCloseKey(key);
    return result;
}

// Registry scanning for JetBrains Rider keys (InstallDir values)
static std::vector<InstallInfo> collect_dotultimate_paths_from_registry(HKEY root, const wchar_t *reg_key) {
    std::vector<InstallInfo> result;
    HKEY key = nullptr;
    if (RegOpenKeyExW(root, reg_key, 0, KEY_READ, &key) != ERROR_SUCCESS) return result;
    auto subkeys = enumerate_subkeys(key);
    for (const auto &name : subkeys) {
        HKEY sub = nullptr;
        if (RegOpenKeyExW(key, name.c_str(), 0, KEY_READ, &sub) != ERROR_SUCCESS) continue;
        std::wstring install_dir_w;
        if (reg_query_string_value(sub, L"InstallDir", install_dir_w) == ERROR_SUCCESS) {
            fs::path exe = fs::path(w2u8(install_dir_w)) / "bin" / "rider64.exe";
            auto info = RiderPathLocator::get_install_info_from_rider_path(exe.string(), InstallInfo::InstallType::Installed);
            if (info.has_value()) result.push_back(*info);
        }
        RegCloseKey(sub);
    }
    RegCloseKey(key);
    return result;
}

// Toolbox location helpers -----------------------------------------------------

static std::string get_toolbox_path_from_env() {
    fs::path p = fs::path(get_env_localappdata()) / "JetBrains" / "Toolbox";
    return p.string();
}

static std::string strip_bin_suffix(const std::string &path) {
    std::regex re("(.*)(?:\\\\|/)bin");
    std::smatch m;
    if (std::regex_search(path, m, re) && m.size() > 1) return m[1].str();
    return {};
}

static std::string get_toolbox_path_from_registry(HKEY root, const wchar_t *reg_key) {
    HKEY key = nullptr;
    if (RegOpenKeyExW(root, reg_key, 0, KEY_READ, &key) != ERROR_SUCCESS) return {};
    std::wstring value;
    std::string result;
    if (reg_query_default_string(key, value) == ERROR_SUCCESS) {
        std::string bin_path = w2u8(value);
        result = strip_bin_suffix(bin_path);
    }
    RegCloseKey(key);
    return result;
}

std::set<InstallInfo, InstallInfoLess> RiderPathLocator::collect_all_paths() {
    std::set<InstallInfo, InstallInfoLess> s;

    // Registry: Uninstall keys
    const wchar_t *uninstall = L"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
    const wchar_t *uninstall_wow = L"SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
    for (auto &i : collect_paths_from_uninstall_registry(HKEY_CURRENT_USER, uninstall)) s.insert(i);
    for (auto &i : collect_paths_from_uninstall_registry(HKEY_LOCAL_MACHINE, uninstall)) s.insert(i);
    for (auto &i : collect_paths_from_uninstall_registry(HKEY_CURRENT_USER, uninstall_wow)) s.insert(i);
    for (auto &i : collect_paths_from_uninstall_registry(HKEY_LOCAL_MACHINE, uninstall_wow)) s.insert(i);

    // Registry: JetBrains Rider keys
    const wchar_t *rider_key = L"SOFTWARE\\JetBrains\\Rider";
    const wchar_t *rider_key_wow = L"SOFTWARE\\WOW6432Node\\JetBrains\\Rider";
    for (auto &i : collect_dotultimate_paths_from_registry(HKEY_CURRENT_USER, rider_key)) s.insert(i);
    for (auto &i : collect_dotultimate_paths_from_registry(HKEY_LOCAL_MACHINE, rider_key)) s.insert(i);
    for (auto &i : collect_dotultimate_paths_from_registry(HKEY_CURRENT_USER, rider_key_wow)) s.insert(i);
    for (auto &i : collect_dotultimate_paths_from_registry(HKEY_LOCAL_MACHINE, rider_key_wow)) s.insert(i);

    // Toolbox: env location
    for (auto &i : get_install_infos_from_toolbox(get_toolbox_path_from_env(), "rider64.exe")) s.insert(i);

    // Toolbox: registry default values
    const wchar_t *tb1 = L"Software\\JetBrains\\Toolbox\\";
    const wchar_t *tb2 = L"Software\\JetBrains s.r.o.\\JetBrainsToolbox\\";
    std::string tb;
    tb = get_toolbox_path_from_registry(HKEY_CURRENT_USER, tb1); if (!tb.empty()) for (auto &i : get_install_infos_from_toolbox(tb, "rider64.exe")) s.insert(i);
    tb = get_toolbox_path_from_registry(HKEY_LOCAL_MACHINE, tb1); if (!tb.empty()) for (auto &i : get_install_infos_from_toolbox(tb, "rider64.exe")) s.insert(i);
    tb = get_toolbox_path_from_registry(HKEY_CURRENT_USER, tb2); if (!tb.empty()) for (auto &i : get_install_infos_from_toolbox(tb, "rider64.exe")) s.insert(i);
    tb = get_toolbox_path_from_registry(HKEY_LOCAL_MACHINE, tb2); if (!tb.empty()) for (auto &i : get_install_infos_from_toolbox(tb, "rider64.exe")) s.insert(i);

    // Resource file, if any (currently noop)
    for (auto &i : get_install_infos_from_resource_file()) s.insert(i);

    return s;
}

#endif // _WIN32


