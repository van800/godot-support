package com.jetbrains.rider.plugins.godot.ideaInterop.fileTypes.tscn

import com.intellij.openapi.fileTypes.FileType
import com.intellij.openapi.fileTypes.SyntaxHighlighter
import com.intellij.openapi.fileTypes.SyntaxHighlighterFactory
import com.intellij.openapi.fileTypes.SyntaxHighlighterProvider
import com.intellij.openapi.project.Project
import com.intellij.openapi.vfs.VirtualFile

class TscnSyntaxHighlighterProvider : SyntaxHighlighterProvider, SyntaxHighlighterFactory() {
    override fun getSyntaxHighlighter(project: Project?, file: VirtualFile?): SyntaxHighlighter {
        return TscnSyntaxHighlighter()
    }

    override fun create(fileType: FileType, project: Project?, file: VirtualFile?): SyntaxHighlighter? {
        if (fileType !is TscnFileType) return null
        return TscnSyntaxHighlighter()
    }
}
