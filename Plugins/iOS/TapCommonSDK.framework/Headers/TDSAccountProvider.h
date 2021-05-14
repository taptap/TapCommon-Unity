//
//  TDSAccountProvider.h
//  TapCommonSDK
//
//  Created by Bottle K on 2021/3/30.
//

#import <TapCommonSDK/TDSAccount.h>

NS_ASSUME_NONNULL_BEGIN

@protocol TDSAccountProvider <NSObject>

- (nullable TDSAccount *)getAccount;
@end

NS_ASSUME_NONNULL_END
