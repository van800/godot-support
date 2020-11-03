package com.jetbrains.rider.plugins.godot.ideaInterop.fileTypes.tscn

import com.intellij.icons.AllIcons
import com.intellij.openapi.fileTypes.LanguageFileType
import com.jetbrains.rider.ideaInterop.fileTypes.RiderLanguageFileTypeBase

object TscnFileType : RiderLanguageFileTypeBase(TscnLanguage) {
    override fun getIcon() = AllIcons.FileTypes.Text
    override fun getName() = "TSCN"
    override fun getDefaultExtension() = "tscn"
    override fun getDescription() = "TSCN file (Godot)"
}
