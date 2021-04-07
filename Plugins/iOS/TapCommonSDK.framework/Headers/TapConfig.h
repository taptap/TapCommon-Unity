//
//  TapConfig.h
//  TapBootstrapSDK
//
//  Created by Bottle K on 2021/2/24.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

typedef NS_ENUM (NSInteger, TapSDKRegionType) {
    TapSDKRegionTypeCN,
    TapSDKRegionTypeIO
};

@interface TapConfig : NSObject
@property (nonatomic, copy) NSString *clientId;
@property (nonatomic, assign) TapSDKRegionType region;
@end

NS_ASSUME_NONNULL_END
