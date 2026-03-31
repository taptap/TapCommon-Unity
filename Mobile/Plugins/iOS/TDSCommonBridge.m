//
//  TDSCommonBridge.m
//  Unity-iPhone
//
//  Created by oneRain on 2023/11/17.
//

#import <Foundation/Foundation.h>
#import "TapCommonSDK/TDSHttpUtil.h"
#import <TapCommonSDK/TDSTracker.h>

const char* TDSCommonBridgeGetDeviceId() {
    return [[TDSHttpUtil getDeviceId] UTF8String];
}

const int TDSCommonBridgeGetDeviceType() {
    return 0;
}

const char* TDSCommonBridgeGetOpenLogCommonParams(int region){
    NSMutableDictionary *common = [TDSTracker commonParams: [TDSTracker baseRequestUrl:region]];
    NSString * config = [[NSString alloc] initWithData:[NSJSONSerialization dataWithJSONObject:common options:NSJSONWritingPrettyPrinted error:nil] encoding:NSUTF8StringEncoding];
    return [config UTF8String];
}
