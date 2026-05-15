#include "rider_locator_gd.h"
#include "rider_path_locator.h"

#include <godot_cpp/classes/engine.hpp>
#include <godot_cpp/core/class_db.hpp>
#include <godot_cpp/variant/dictionary.hpp>

using namespace godot;

void RiderLocator::_bind_methods() {
    ClassDB::bind_method(D_METHOD("get_installations"), &RiderLocator::get_installations);
}

static String support_to_string(InstallInfo::SupportUproject s) {
    using S = InstallInfo::SupportUproject;
    switch (s) {
        case S::None: return String("None");
        case S::Beta: return String("Beta");
        case S::Release: return String("Release");
    }
    return String("None");
}

static String type_to_string(InstallInfo::InstallType t) {
    using T = InstallInfo::InstallType;
    switch (t) {
        case T::Installed: return String("Installed");
        case T::Toolbox: return String("Toolbox");
        case T::Custom: return String("Custom");
    }
    return String("Installed");
}

Array RiderLocator::get_installations() const {
    Array result;
    std::set<InstallInfo, InstallInfoLess> set = RiderPathLocator::collect_all_paths();

    for (const auto &info : set) {
        Dictionary d;
        const std::string &p = info.path;
        // compose version string
        String ver;
        for (size_t i = 0; i < info.version.parts.size(); ++i) {
            if (i > 0) ver += ".";
            ver += String::num_int64(info.version.parts[i]);
        }
        d["path"] = String(p.c_str());
        d["version"] = ver;
        d["support"] = support_to_string(info.support);
        d["type"] = type_to_string(info.type);
        // Avoid String + Variant ambiguity: use the raw path string
        const String display = String("Rider ") + ver + String(" ") + String(p.c_str());
        d["display"] = display;
        result.push_back(d);
    }

    return result;
}
