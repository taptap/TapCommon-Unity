//
//  TDSCommonService.h
//  TDSCommon
//
//  Created by TapTap-David on 2020/11/10.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSCommonService : NSObject

+ (void)useNativeDataInCore:(NSNumber *)enable;

+ (void)setDurationStatisticsEnabled:(NSNumber *)enable;

+ (void)initWithConfig:(NSString*)configJSON versionName:(NSString*)versionName;

+ (void)setXUA:(NSString*)json;

+ (void)getRegionCode:(void (^)(NSString *result))callback;

+ (void)getDeviceId:(void (^)(NSString *result))callback;

+ (void)isTapTapInstalled:(void (^)(NSString *result))callback;

+ (void)isTapGlobalInstalled:(void (^)(NSString *result))callback;

+ (void)preferredLanguage:(NSNumber *)language;

+ (void)hostToBeReplaced:(NSString*)host replacedHost:(NSString*) replaceHost;

+ (void)setCurrentTdsId:(NSString *)tdsId;

@end

NS_ASSUME_NONNULL_END
