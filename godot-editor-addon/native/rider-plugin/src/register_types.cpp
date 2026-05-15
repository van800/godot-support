#include "register_types.h"

#include <gdextension_interface.h>
#include <godot_cpp/core/class_db.hpp>
#include <godot_cpp/core/defs.hpp>
#include <godot_cpp/godot.hpp>
#include "rider_locator_gd.h"

using namespace godot;

void initialize_gdextension_types(ModuleInitializationLevel p_level)
{
    if (p_level == MODULE_INITIALIZATION_LEVEL_EDITOR) {
        ClassDB::register_class<RiderLocator>();
    }
}

void uninitialize_gdextension_types(ModuleInitializationLevel p_level) {
	// Nothing to do for now.
}

extern "C"
{
	// Initialization
	GDExtensionBool GDE_EXPORT rider_library_init(GDExtensionInterfaceGetProcAddress p_get_proc_address, GDExtensionClassLibraryPtr p_library, GDExtensionInitialization *r_initialization)
	{
		GDExtensionBinding::InitObject init_obj(p_get_proc_address, p_library, r_initialization);
		init_obj.register_initializer(initialize_gdextension_types);
		init_obj.register_terminator(uninitialize_gdextension_types);
		// We need editor-level initialization for EditorPlugin classes.
		init_obj.set_minimum_library_initialization_level(MODULE_INITIALIZATION_LEVEL_EDITOR);

		return init_obj.init();
	}
}