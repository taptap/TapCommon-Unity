//
//  TDSSDK.h
//  TDSCommon
//
//  Created by Bottle K on 2020/10/13.
//

#import <Foundation/Foundation.h>
#import <TapCommonSDK/TDSAccount.h>

#define TDS_COMMON_VERSION @"1.1.11"
#define TDS_COMMON_VERSION_NUMBER @"11"

NS_ASSUME_NONNULL_BEGIN
typedef NSString *TDSLanguage NS_STRING_ENUM;

FOUNDATION_EXPORT TDSLanguage const TDSLanguageCN;
FOUNDATION_EXPORT TDSLanguage const TDSLanguageEN;

@interface TDSBaseManager : NSObject

+ (instancetype)new NS_UNAVAILABLE;

+ (instancetype)shareInstance;

- (void)setLanguage:(NSString *)language;

- (NSString *)getLanguage;
@end

NS_ASSUME_NONNULL_END
