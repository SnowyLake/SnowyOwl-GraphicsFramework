# SnowyOwl GraphicsFramework

## 目录

- [简介](#简介)
- [相关仓库](#相关仓库)
- [仓库结构](#仓库结构)
- [环境要求](#环境要求)

## 简介

SnowyOwl GraphicsFramework 是一个面向移动平台高质量渲染的 Unity 图形框架, 基于定制的 Universal Render Pipeline 构建. 本仓库使用 monorepo 管理框架及其依赖的 CoreRP 和 URP fork.

## 相关仓库

- [SnowyOwl Samples](https://github.com/SnowyLake/SnowyOwl-Samples): 使用本框架的 Unity 示例项目.
- [SnowyOwl GraphicsFramework](https://github.com/SnowyLake/SnowyOwl-GraphicsFramework): 当前框架仓库.

## 仓库结构

```text
Packages/
├─ com.snowyowl.graphicsframework/
├─ com.unity.render-pipelines.core/
└─ com.unity.render-pipelines.universal/
```

- `com.snowyowl.graphicsframework` 包含 SnowyOwl GraphicsFramework 的运行时, 编辑器和 Shader 实现.
- `com.unity.render-pipelines.core` 基于 Unity SRP Core `14.0.12` 定制.
- `com.unity.render-pipelines.universal` 基于 Unity URP `14.0.12` 定制.

## 环境要求

- Unity `2022.3+`
- Odin Inspector

本仓库不包含 Odin Inspector. 请在引入 SnowyOwl GraphicsFramework 前自行引入该插件, 否则引用 `Sirenix.OdinInspector` 的代码将无法编译.
