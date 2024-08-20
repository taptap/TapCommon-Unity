//
//  TDSSDK.h
//  TDSCommon
//
//  Created by Bottle K on 2020/10/13.
//

#import <Foundation/Foundation.h>
#import <TapCommonSDK/TapConfig.h>

#define TapCommonSDK @"TapCommon"
#define TapCommonSDK_VERSION_NUMBER @"32902001"
#define TapCommonSDK_VERSION        @"3.29.2"

NS_ASSUME_NONNULL_BEGIN

@interface TDSBaseManager : NSObject

+ (TDSBaseManager *)shareInstance;
+ (void)setDurationStatisticsEnabled:(BOOL)enable;
+ (void)setNativeCoreEnabled:(BOOL)enable;
+ (void)initWithSDKConfig:(TapConfig *)config;
+ (TapConfig *)getConfig;
+ (void)setDurationExtraParams:(NSDictionary<NSString *, NSObject *> *) params;

@end

NS_ASSUME_NONNULL_END
