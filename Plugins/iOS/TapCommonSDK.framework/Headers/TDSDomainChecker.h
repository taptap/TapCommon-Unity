//
//  TDSDomainChecker.h
//  TapCommonSDK
//
//  Created by Bottle K on 2021/4/19.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSDomainChecker : NSObject

/// 配置日志参数
/// @param clientId clientId
/// @param clientSecret clientSecret
/// @param sdkVersionName SDK Version Name
+ (void)setupTrackerWithId:(NSString *)clientId secret:(NSString *)clientSecret sdkVersionName:(NSString *)sdkVersionName;

/// 配置域名
+ (void)setupDomains;

/// 获取一个当前可用域名
+ (NSString *)getActiveDomain;

/// 标记一个域名不可用
/// @param domain 域名
+ (void)deactiveDomain:(NSString *)domain;

/// 开始检测域名
+ (void)startCheckDomains;

+ (void)stopCheckDomains;
@end

NS_ASSUME_NONNULL_END
