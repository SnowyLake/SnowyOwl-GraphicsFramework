# SnowyOwl GraphicsFramework Agent Guide

## 目录

- [仓库边界](#仓库边界)
- [实现约束](#实现约束)
- [验证入口](#验证入口)

## 仓库边界

- `Packages/com.snowyowl.graphicsframework/` 是框架主体.
- `Packages/com.unity.render-pipelines.core/` 和 `Packages/com.unity.render-pipelines.universal/` 是基于 Unity SRP `14.0.12` 的配套 fork, 三个 package 必须保持兼容.
- `Packages/com.snowyowl.graphicsframework/ThirdParty/` 是随包引入的第三方代码, 除非任务明确涉及, 不要修改.

## 实现约束

- 框架功能优先放入 `com.snowyowl.graphicsframework`, 只有公共渲染管线无法提供所需接入点时才修改 CoreRP 或 URP fork.
- 修改跨 package API 时, 同步检查另外两个 package 的调用方和程序集引用.
- `Shaders/Include/Generated/` 中的文件由生成流程维护, 应修改对应生成器或输入, 不要只手工修改生成结果.

## 验证入口

- 本仓库不是完整 Unity 工程. 使用 SnowyOwl Samples 工程打开和验证改动.
- Samples 同时引用三个本地 package, 不需要复制 package 文件.
- 使用 Unity `2022.3.62f3` 验证编译, Shader 导入和受影响的示例场景.
