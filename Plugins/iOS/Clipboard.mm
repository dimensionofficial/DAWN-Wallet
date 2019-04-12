#import "Clipboard.h"  
@implementation Clipboard  
//将文本复制到IOS剪贴板  
- (void)objc_copyTextToClipboard : (NSString *)text  
{  
    UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];  
    pasteboard.string = text;  
}  
@end  
extern "C" {
     static Clipboard *iosClipboard;
   
     void _copyTextToClipboard(const char *textList)
    {   
        NSString *text = [NSString stringWithUTF8String: textList] ;
       
        if(iosClipboard == NULL)
        {
            iosClipboard = [[Clipboard alloc] init];
        }
       
        [iosClipboard objc_copyTextToClipboard: text];
    }
    
    int _getStatuBarIOS()
    {
        CGRect rectOfStatusbar = [[UIApplication sharedApplication] statusBarFrame];
        float scale = [[UIScreen mainScreen] scale];
        return rectOfStatusbar.size.height * scale;
    }
    
    int _getPointPx()
    {
        float scale = [[UIScreen mainScreen] scale];
        return scale;
    }

}
