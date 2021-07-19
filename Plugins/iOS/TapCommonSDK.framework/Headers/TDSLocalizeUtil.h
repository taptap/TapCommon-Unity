//
//  TDSLocalizeUtil.h
//  TDSCommon
//
//  Created by Bottle K on 2021/3/4.
//

#import <Foundation/Foundation.h>
#import "TDSLocalizeManager.h"

NS_ASSUME_NONNULL_BEGIN

@interface TDSLocalizeUtil : NSObject

+ (instancetype)shareInstance;

+ (void)setCurrentLanguage:(TapLanguageType)langType;

+ (TapLanguageType)getCurrentLangType;

+ (NSString *)getCurrentLangString;

+ (NSDictionary *)readJsonFile:(NSString *)filePath;

+ (NSString *)getTextWithLangDic:(NSDictionary *)dic byKey:(NSString *)key;
@end

NS_ASSUME_NONNULL_END
