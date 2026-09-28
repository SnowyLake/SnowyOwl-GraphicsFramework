# SnowyOwl GraphicsFramework Agent Guide

## 目录

- [仓库边界](#仓库边界)
- [实现约束](#实现约束)
- [宏与 Shader 参数](#宏与-shader-参数)
- [验证入口](#验证入口)

## 仓库边界

- `Packages/com.snowyowl.graphicsframework/` 是框架主体.
- `Packages/com.unity.render-pipelines.core/` 和 `Packages/com.unity.render-pipelines.universal/` 是基于 Unity SRP `14.0.12` 的配套 fork, 三个 package 必须保持兼容.
- `Packages/com.snowyowl.graphicsframework/ThirdParty/` 是随包引入的第三方代码, 除非任务明确涉及, 不要修改.

## 实现约束

- 框架功能优先放入 `com.snowyowl.graphicsframework`, 只有公共渲染管线无法提供所需接入点时才修改 CoreRP 或 URP fork.
- CoreRP/URP fork 的定制代码必须用 `SNOWYOWL_INCLUDE` 隔离; 去掉宏后应恢复 Unity SRP `14.0.12` 的上游行为. 框架自身代码不以此宏添加兼容守卫. 现有 README 和 package `displayName` 修改为保留的非代码例外.
- 修改跨 package API 时, 同步检查另外两个 package 的调用方和程序集引用.
- `Shaders/Include/Generated/` 中的文件由生成流程维护, 应修改对应生成器或输入, 不要只手工修改生成结果.

## 宏与 Shader 参数

- `Editor/SnowyOwlScriptingDefineInstaller.cs` 在编辑器启动和切换构建目标时为当前目标补齐 C# `SNOWYOWL_INCLUDE`. Batchmode 首次设置宏不会在同次启动中自动重编译脚本; CI 应在首次启动 Unity 前为目标平台预置宏.
- HLSL 的 `SNOWYOWL_INCLUDE` 独立于 C# 宏, 固定定义在 `Shaders/Include/Common/CommonIncludes.hlsl`. 框架 shader 通过 `PassIncludes.hlsl` 的 `#include_with_pragmas` 引入该文件; 普通 URP shader 不会自动获得此宏. Shader 定义头不由设置资产生成或改写文本.
- `CommonIncludes.hlsl` 声明 `_DEPTH_PRIMING_ON` 变体. `SetupShaderParametersRendererFeature.SetupRenderPasses` 在 URP 完成相机设置后读取实际 depth priming 状态并设置关键字, 同时按 `enable` 和 `cameraTypes` 设置角色光照 Volume 全局参数. `SetupShaderParametersPerCamera.cs` 的 `beginCameraRendering` 回调只清除前一相机的关键字; 此时 Volume 尚未更新, 也不能判断当前相机的实际 depth priming 状态.

## 验证入口

- 本仓库不是完整 Unity 工程. 使用 SnowyOwl Samples 工程打开和验证改动.
- Samples 同时引用三个本地 package, 不需要复制 package 文件.
- Unity 交互统一使用 `uloop` CLI, 在 Samples 工程中用 Unity `2022.3.62f3` 验证编译, Shader 导入和受影响的示例场景.
