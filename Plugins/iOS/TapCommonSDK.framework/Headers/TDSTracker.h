//
//  TDSTracker.h
//  TDSCommon
//
//  Created by TapTap-David on 2021/1/15.
//

#import <Foundation/Foundation.h>
#import "TDSTrackerConfig.h"

NS_ASSUME_NONNULL_BEGIN

@interface TDSTracker : NSObject

- (instancetype)initWithConfig:(TDSTrackerConfig *)config;

- (void)track:(NSDictionary *)logContentsMap;

- (NSMutableDictionary *)commonParams:(NSString *) clientId version:(NSString *) version versionCode:(NSString *) versionCode baseUrl:(NSString *)baseUrl;
+ (NSMutableDictionary *)commonParams:(NSString *) baseUrl;
// openLog 请求链接 region: 区域 0 国内 1 海外 2 rnd
+ (NSString *)baseRequestUrl:(int) region;

@end

NS_ASSUME_NONNULL_END
