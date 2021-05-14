//
//  TDSSDK.h
//  TDSCommon
//
//  Created by Bottle K on 2020/10/13.
//

#import <Foundation/Foundation.h>
#import <TapCommonSDK/TDSAccount.h>

#define TapCommonSDK @"TapCommon"
#define TapCommonSDK_VERSION_NUMBER @"20102001"
#define TapCommonSDK_VERSION        @"2.1.2"

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
