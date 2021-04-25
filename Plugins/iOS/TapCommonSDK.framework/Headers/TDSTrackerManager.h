//
//  TDSTrackerManager.h
//  TDSCommon
//
//  Created by TapTap-David on 2021/1/19.
//

#import <Foundation/Foundation.h>
#import <TapCommonSDK/TDSTrackerConfig.h>
#import <TapCommonSDK/UserModel.h>
#import <TapCommonSDK/PageModel.h>
#import <TapCommonSDK/ActionModel.h>
#import <TapCommonSDK/NetworkStateModel.h>

NS_ASSUME_NONNULL_BEGIN

@interface TDSTrackerManager : NSObject

+ (instancetype)sharedInstance;

+ (void)registerTracker:(TDSTrackerConfig *)trackerConfig;

- (void)trackerWithType:(TDSTrackerType)trackerType
              userModel:(nullable UserModel *)userModel
              pageModel:(nullable PageModel *)pageModel
            actionModel:(nullable ActionModel *)actionModel
           networkModel:(nullable NetworkStateModel *)networkModel;

@end

NS_ASSUME_NONNULL_END
