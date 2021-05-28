## [TapTap.Common](./Documentation/README.md)

### 接口描述

#### 1.获取地区

```c#
TapCommon.GetRegionCode(isMainland =>
{
    // true 中国大陆 false 非中国大陆
});
```

#### 2. TapTap 是否安装
```c#
TapCommon.IsTapTapInstalled(installed =>
{
    // true 安装 false 未安装
});
```

#### 3. TapTap IO 是否安装
```c#
TapCommon.IsTapTapGlobalInstalled(installed =>
{
    // true 安装  false 未安装
});
```

### Android 独占方法

#### 4. 在 TapTap 更新游戏
```c#
TapCommon.UpdateGameInTapTap(appId,updateSuccess =>
{
    // true 更新成功 false 更新失败
});
```

#### 5. 在 TapTap IO 更新游戏
```c#
TapCommon.UpdateGameInTapGlobal(appId,updateSuccess =>
{
    // true 更新成功 false 更新失败
});
```

#### 6. 在 TapTap 打开当前游戏的评论区
```c#
TapCommon.OpenReviewInTapTap(appId,openSuccess =>
{
    // true 打开评论区 false 打开失败
});
```

#### 6. 在 TapTap IO 打开当前游戏的评论区
```c#
TapCommon.openReviewInTapGlobal(appId,openSuccess =>
{
    // true 打开评论区 false 打开失败
});
```