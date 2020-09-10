package com.jetbrains.rider.plugins.godot.run.configurations

import com.intellij.execution.Executor
import com.intellij.execution.configurations.ConfigurationFactory
import com.intellij.execution.configurations.ConfigurationTypeUtil
import com.intellij.execution.configurations.RunProfileState
import com.intellij.execution.executors.DefaultDebugExecutor
import com.intellij.execution.runners.ExecutionEnvironment
import com.intellij.openapi.project.Project
import com.jetbrains.rdclient.util.idea.toIOFile
import com.jetbrains.rider.plugins.godot.GodotProjectDiscoverer
import com.jetbrains.rider.plugins.godot.GodotServer
import com.jetbrains.rider.run.configurations.exe.ExeConfiguration
import com.jetbrains.rider.run.configurations.exe.ExeConfigurationParameters
import com.jetbrains.rider.run.configurations.remote.DotNetRemoteConfiguration
import com.jetbrains.rider.run.configurations.remote.MonoRemoteConfigType

class GodotDebugRunConfiguration(name:String, project: Project, factory: ConfigurationFactory, params: ExeConfigurationParameters)
    : ExeConfiguration(name, project, factory, params) {

    override fun isNative(): Boolean {
        return false
    }

    override fun getState(executor: Executor, environment: ExecutionEnvironment): RunProfileState{
        val executorId = executor.id

        if (executorId == DefaultDebugExecutor.EXECUTOR_ID)
            return GodotDebugProfileState(this, DotNetRemoteConfiguration(project, ConfigurationTypeUtil.findConfigurationType(MonoRemoteConfigType::class.java).factory, name), environment)

        return super.getState(executor, environment)
    }

    override fun checkConfiguration() {
        val godotPathProperty = GodotProjectDiscoverer.getInstance(project).godotPath
        if (!parameters.exePath.toIOFile().exists()){
            val path = GodotServer.getGodotPath(project)
            if (path != null){
                if (path.toIOFile().exists()){
                    godotPathProperty.set(path)
                }
            }
        }

        parameters.exePath = godotPathProperty.value ?: ""

        super.checkConfiguration()
    }
}