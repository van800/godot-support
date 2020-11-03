package com.jetbrains.rider.plugins.godot.run.configurations

import com.intellij.execution.RunManager
import com.intellij.execution.actions.ConfigurationContext
import com.intellij.execution.actions.LazyRunConfigurationProducer
import com.intellij.execution.configurations.runConfigurationType
import com.intellij.openapi.roots.ProjectRootManager
import com.intellij.openapi.util.Ref
import com.intellij.openapi.util.io.FileUtil
import com.intellij.psi.PsiElement
import com.intellij.psi.PsiFile
import com.jetbrains.rider.plugins.godot.GodotProjectDiscoverer
import com.jetbrains.rider.plugins.godot.ideaInterop.fileTypes.tscn.TscnLanguage
import com.jetbrains.rider.plugins.godot.run.GodotRunConfigurationGenerator
import java.io.File

class DebugSceneRunConfigurationProducer : LazyRunConfigurationProducer<GodotDebugRunConfiguration>() {
    override fun getConfigurationFactory() = runConfigurationType<GodotDebugRunConfigurationType>().factory

    override fun isConfigurationFromContext(configuration: GodotDebugRunConfiguration, context: ConfigurationContext): Boolean {
        if (!GodotProjectDiscoverer.getInstance(context.project).isGodotProject.value) return false

        val resPath = extractResPath(context) ?: return false
        return configuration.parameters.programParameters.contains(resPath)
    }

    override fun setupConfigurationFromContext(configuration: GodotDebugRunConfiguration,
                                               context: ConfigurationContext,
                                               sourceElement: Ref<PsiElement>): Boolean {
        val file = getContainingFile(context) ?: return false
        if (!isSceneFile(file)) return false
        val resPath = extractResPath(context) ?: return false

        val path = GodotProjectDiscoverer.getInstance(context.project).godotPath.value
        if (path == null || !File(path).exists()) {
            val runManager = RunManager.getInstance(context.project)
            val playerSettings = runManager.allSettings.firstOrNull { it.type is GodotDebugRunConfigurationType && it.name == GodotRunConfigurationGenerator.PLAYER_CONFIGURATION_NAME }
                    ?: return false
            val config = playerSettings.configuration as GodotDebugRunConfiguration
            configuration.parameters.exePath = config.parameters.exePath
        }
        configuration.parameters.exePath = path!!
        configuration.parameters.programParameters = "--path \"${context.project.basePath}\" \"$resPath\""

        configuration.parameters.workingDirectory = "${context.project.basePath}"
        configuration.name = file.name
        return true
    }

    private fun extractResPath(context: ConfigurationContext): String? {
        val file = getContainingFile(context) ?: return null
        val project = context.project ?: return null
        val root = ProjectRootManager.getInstance(project).fileIndex.getContentRootForFile(file.virtualFile) ?: return null
        val filename = FileUtil.getRelativePath(root.path, file.virtualFile.path, '/')
        return "res://$filename"
    }

    private fun isSceneFile(file: PsiFile): Boolean {
        // TODO: Proper scene detection that does not rely on the extension
        return file.language is TscnLanguage && file.virtualFile.extension == "tscn"
    }

    private fun getContainingFile(context: ConfigurationContext): PsiFile? {
        val location = context.psiLocation ?: return null
        return location.containingFile
    }
}
