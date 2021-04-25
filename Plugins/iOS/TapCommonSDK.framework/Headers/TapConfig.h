//
//  TapConfig.h
//  TapBootstrapSDK
//
//  Created by Bottle K on 2021/2/24.
//

#import <Foundation/Foundation.h>
#import <TapCommonSDK/TapDBConfig.h>

NS_ASSUME_NONNULL_BEGIN

typedef NS_ENUM (NSInteger, TapSDKRegionType) {
    TapSDKRegionTypeCN,
    TapSDKRegionTypeIO
};

@interface TapConfig : NSObject
@property (nonatomic, copy) NSString *clientId;
@property (nonatomic, copy) NSString *clientSecret;
@property (nonatomic, assign) TapSDKRegionType region;
@property (nonatomic, strong) TapDBConfig * dbConfig;
@end

NS_ASSUME_NONNULL_END
