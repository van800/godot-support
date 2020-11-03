package com.jetbrains.rider.plugins.godot.ideaInterop.fileTypes.tscn

import com.intellij.lexer.DummyLexer
import com.intellij.openapi.fileTypes.PlainTextParserDefinition
import com.intellij.openapi.project.Project
import com.intellij.psi.FileViewProvider
import com.intellij.psi.tree.IElementType
import com.jetbrains.rider.ideaInterop.fileTypes.RiderFileElementType
import com.jetbrains.rider.ideaInterop.fileTypes.RiderParserDefinitionBase

class TscnParserDefinition : RiderParserDefinitionBase(TscnFileElementType, TscnFileType) {
    companion object {
        val TscnElementType = IElementType("RIDER_TSCN", TscnLanguage)
        val TscnFileElementType = RiderFileElementType("RIDER_TSCN_FILE", TscnLanguage, TscnElementType)
    }

    override fun createLexer(project: Project?) = DummyLexer(TscnFileElementType)
}