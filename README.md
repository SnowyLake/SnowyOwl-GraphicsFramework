# SnowyOwl GraphicsFramework

## 目录

- [简介](#简介)
- [仓库结构](#仓库结构)
- [环境要求](#环境要求)
- [安装](#安装)
- [本地开发](#本地开发)
- [许可证](#许可证)

## 简介

SnowyOwl GraphicsFramework 是一个面向移动平台高质量渲染的 Unity 图形框架, 基于定制的 Universal Render Pipeline 构建. 本仓库使用 monorepo 管理框架及其依赖的 CoreRP 和 URP fork.

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

- Unity `2022.3.62f3`.
- Git `2.14.0` 或更高版本.

## 安装

由于框架使用定制的 CoreRP 和 URP, 项目必须在 `Packages/manifest.json` 中同时声明三个 package:

```json
{
  "dependencies": {
    "com.snowyowl.graphicsframework": "https://github.com/SnowyLake/SnowyOwl-GraphicsFramework.git?path=/Packages/com.snowyowl.graphicsframework#develop",
    "com.unity.render-pipelines.core": "https://github.com/SnowyLake/SnowyOwl-GraphicsFramework.git?path=/Packages/com.unity.render-pipelines.core#develop",
    "com.unity.render-pipelines.universal": "https://github.com/SnowyLake/SnowyOwl-GraphicsFramework.git?path=/Packages/com.unity.render-pipelines.universal#develop"
  }
}
```

`develop` 用于持续开发. 稳定项目应将三个引用固定到同一个 release tag 或完整 commit SHA.

## 本地开发

将本仓库与 Unity 工程放在同一父目录, 然后使用本地 `file:` 依赖:

```json
{
  "dependencies": {
    "com.snowyowl.graphicsframework": "file:../../SnowyOwl-GraphicsFramework/Packages/com.snowyowl.graphicsframework",
    "com.unity.render-pipelines.core": "file:../../SnowyOwl-GraphicsFramework/Packages/com.unity.render-pipelines.core",
    "com.unity.render-pipelines.universal": "file:../../SnowyOwl-GraphicsFramework/Packages/com.unity.render-pipelines.universal"
  }
}
```

使用本地依赖时, C#, Shader 和资源修改会由 Unity 直接重新编译或导入.

## 许可证

本仓库包含多个独立授权的 package. SnowyOwl GraphicsFramework 使用 MIT License, CoreRP 和 URP 使用 Unity Companion License. 详细信息参见根目录 `LICENSE` 和各 package 自带的许可证文件.
