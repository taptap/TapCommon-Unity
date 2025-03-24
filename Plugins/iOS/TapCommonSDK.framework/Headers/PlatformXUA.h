//
//  PlatformXUA.h
//  TapCommonSDK
//
//  Created by Bottle K on 2021/6/21.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface PlatformXUA : NSObject
@property (nonatomic, copy) NSDictionary *xuaMap;
@property (nonatomic, copy) NSString *engineTdsId;

+ (instancetype)shareInstance;
// 设置 unity 端 tdsId
- (void) setCurrentTdsId:(NSString *) tdsId;

+ (NSString *) getTrackXUA;

@end

NS_ASSUME_NONNULL_END
