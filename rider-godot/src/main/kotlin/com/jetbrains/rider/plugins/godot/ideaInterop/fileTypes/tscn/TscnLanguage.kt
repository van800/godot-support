package com.jetbrains.rider.plugins.godot.ideaInterop.fileTypes.tscn

import com.jetbrains.rider.ideaInterop.fileTypes.RiderLanguageBase

object TscnLanguage : RiderLanguageBase("TSCN", "TSCN") {
    override fun isCaseSensitive(): Boolean = true
}
