using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HardwareWallet
{
    public enum Words
    {
        设置密码,
        下一步,
        确认密码,
        确定,
        设置手势锁,
        至少经过四个点,
        确认手势锁,
        新建与恢复,
        创建账户,
        恢复账户,
        新建币种,
        配对,
        地址,
        签名,
        签名交易,
        请打开DRACO应用扫一扫,
        币种管理,
        手势锁,
        锁屏时间,
        备份,
        语言切换,
        亮度调节,
        清除数据,
        助记词说明,
        设置,
        我知道了,
        返回,
        我已经抄录下来了,
        请记住以下助记词,
        确认助记词,
        助记词用于恢复钱包和重置签保密请一定要抄录下来并妥善保管好,
        扫描二维码完成签名,
        密码错误,
        请输入6位数字密码,
        两次输入的密码不相同,
        两次手势不相同,
        内部错误,
        助记词错误,
        恢复出厂设置,
        当前手势锁,
        设置新手势锁,
        确认新手势锁,
        手势锁修改成功,
        手势解锁,
        设备停用,
        不能识别的二维码,
        重复密码,
        请重复6位数字密码,
        请输入手势锁,
        请重复手势锁,
        DracoWallet,
        欢迎,
        设置密码1,
        手势锁2,
        创建OR恢复,
        钱包命名,
        钱包恢复,
        钱包创建完成,
        如您是首次使用钱包,
        如您曾通过本产品创建数字钱包,
        请输入助记词,
        输入时请在每两个词之间加上空格分隔,
        请勿拍摄并确保无人观看,
        助记词用于恢复钱包和重置密码,
        今后不再显示,
        hi欢迎使用天龙钱包,
        我们将为您提供安全与简单的,
        开始设置,
        钱包未备份,
        请为您的钱包设置由4位数组成的名称,
        创建成功,
        请舒勇DRACOAPP配对,
        接收钱款,
        付款签名,
        设备配对,
        秒,
        秒30,
        分钟1,
        分钟2,
        分钟5,
        永不,
        扫二维码完成签名,
        签名信息,
        币种,
        矿工费,
        数量,
        接收地址,
        发送地址,
        确认签名后打开热钱包扫描,
        确认签名,
        取消,
        签名支付,
        请在手机上打开DRACO扫一扫,
        上一张,
        下一张,
        输入密码,
        错误,
        扫描完成,
        请扫描下一张二维码,
        此二维码已扫过,
        提示,
        创建中,
        关机,
        重启
    }
    public class Language
    {
        public List<string> labels;
        public string GetWord(Words words)
        {
            return labels[(int)words];
        }
    }

    public class SimpleChinese : Language
    {
        public SimpleChinese()
        {
            labels = new List<string>();
            labels.Add("设置密码");
            labels.Add("下一步");
            labels.Add("确认密码");
            labels.Add("确  定");
            labels.Add("设置手势锁");
            labels.Add("至少经过四个点");
            labels.Add("确认手势锁");
            labels.Add("新建与恢复");
            labels.Add("创建钱包");
            labels.Add("恢复钱包");
            labels.Add("新建币种");
            labels.Add("配对");
            labels.Add("收款地址");
            labels.Add("签名");
            labels.Add("签名交易");
            labels.Add("请在手机上打开\r\ndimension应用扫一扫");
            labels.Add("币种管理");
            labels.Add("修改手势锁");
            labels.Add("锁屏时间");
            labels.Add("钱包备份");
            labels.Add("语言切换");
            labels.Add("亮度调节");
            labels.Add("恢复出厂设置");
            labels.Add("助记词说明");
            labels.Add("设置");
            labels.Add("我知道了");
            labels.Add("返回");
            labels.Add("我已经抄录下来了");
            labels.Add("请记住以下助记词");
            labels.Add("确认助记词");
            labels.Add("助记词用于恢复钱包和重置钱包密码，请一定要抄录下来并妥善保管好");
            labels.Add("扫描二维码完成签名");
            labels.Add("密码错误");
            labels.Add("请输入8位数字密码");
            labels.Add("两次输入的密码不相同");
            labels.Add("两次手势不相同");
            labels.Add("内部错误");
            labels.Add("助记词错误");
            labels.Add("恢复出厂设置");
            labels.Add("当前手势锁");
            labels.Add("设置新手势锁");
            labels.Add("确认新手势锁");
            labels.Add("手势锁修改成功");
            labels.Add("手势解锁");
            labels.Add("设备停用，剩余时间：{0}秒");
            labels.Add("不能识别的二维码");
            labels.Add("重复密码");
            labels.Add("请重复8位数字密码");
            labels.Add("请输入手势锁");
            labels.Add("请重复手势锁");
            labels.Add("Dimension 钱包");
            labels.Add("欢迎");
            labels.Add("1.设置密码");
            labels.Add("2.手势锁");
            labels.Add("3.创建OR恢复");
            labels.Add("4.钱包命名");
            labels.Add("钱包恢复");
            labels.Add("钱包创建完成");
            labels.Add("如您是首次使用数字钱包，请选择该项");
            labels.Add("如您曾通过本产品创建数字钱包，可选择此项进行助记词恢复");
            labels.Add("请输入助记词");
            labels.Add("输入时，请在每两个词之间加上空格分隔");
            labels.Add("请勿拍摄并确保无人观看");
            labels.Add("助记词用于恢复钱包和重置钱包密码，请一定要抄录下来并妥善保管好。");
            labels.Add("今后不再显示");
            labels.Add("Hi，欢迎使用Dimension\r\nDimension数字钱包");
            labels.Add("我们将为您提供安全与简单的数字资产保护方案。");
            labels.Add("开始设置");
            labels.Add("钱包未备份");
            labels.Add("请为您的钱包设置\r\n由4位数字组成的名称");
            labels.Add("创建成功！");
            labels.Add("请使用dimension APP配对后使用");
            labels.Add("         接收钱款");
            labels.Add("         付款签名");
            labels.Add("         设备配对");
            labels.Add("秒");
            labels.Add("30秒");
            labels.Add("1分钟");
            labels.Add("2分钟");
            labels.Add("5分钟");
            labels.Add("永不");
            labels.Add("扫二维码完成签名");
            labels.Add("签名信息");
            labels.Add("币种");
            labels.Add("矿工费");
            labels.Add("数量");
            labels.Add("接收地址");
            labels.Add("发送地址");
            labels.Add("确认签名后，请打开热钱包扫描签名后的二维码");
            labels.Add("确认签名");
            labels.Add("取  消");
            labels.Add("签名支付");
            labels.Add("请在手机上打开dimension扫一扫");
            labels.Add("上一张");
            labels.Add("下一张");
            labels.Add("输入密码");
            labels.Add("错误");
            labels.Add("扫描完成\r\n");
            labels.Add("请扫描下一张二维码");
            labels.Add("此二维码已扫过，请扫描下一张二维码");
            labels.Add("提示");
            labels.Add("创建中...");
            labels.Add("关机");
            labels.Add("重启");
        }
    }

    public class TWChinese : Language
    {
        public TWChinese()
        {
            labels = new List<string>();
            labels.Add("設置密碼");
            labels.Add("下一步");
            labels.Add("確認密碼");
            labels.Add("確  定");
            labels.Add("設置手勢鎖");
            labels.Add("至少經過四個點");
            labels.Add("確認手勢鎖");
            labels.Add("新建與恢復");
            labels.Add("創建錢包");
            labels.Add("恢復錢包");
            labels.Add("新建幣種");
            labels.Add("配對");
            labels.Add("收款地址");
            labels.Add("簽名");
            labels.Add("簽名交易");
            labels.Add("請在手機上打開\r\ndimension應用掃一掃");
            labels.Add("幣種管理");
            labels.Add("修改手勢鎖");
            labels.Add("鎖屏時間");
            labels.Add("錢包備份");
            labels.Add("語言切換");
            labels.Add("亮度調節");
            labels.Add("恢復出廠設置");
            labels.Add("助記詞說明");
            labels.Add("設置");
            labels.Add("我知道了");
            labels.Add("返回");
            labels.Add("我已經抄錄下來了");
            labels.Add("請記住以下助記詞");
            labels.Add("確認助記詞");
            labels.Add("助記詞用於恢復錢包和重置錢包密碼，請一定要抄錄下來並妥善保管好");
            labels.Add("掃描二維碼完成簽名");
            labels.Add("密碼錯誤");
            labels.Add("請輸入8位元數位密碼");
            labels.Add("兩次輸入的密碼不相同");
            labels.Add("兩次手勢不相同");
            labels.Add("內部錯誤");
            labels.Add("助記詞錯誤");
            labels.Add("恢復出廠設置");
            labels.Add("當前手勢鎖");
            labels.Add("設置新手勢鎖");
            labels.Add("確認新手勢鎖");
            labels.Add("手勢鎖修改成功");
            labels.Add("手勢解鎖");
            labels.Add("設備停用，剩餘時間：{0}秒");
            labels.Add("不能識別的二維碼");
            labels.Add("重複密碼");
            labels.Add("請重複8位元數位密碼");
            labels.Add("請輸入手勢鎖");
            labels.Add("請重複手勢鎖");
            labels.Add("大師錢包");
            labels.Add("歡迎");
            labels.Add("1.設置密碼");
            labels.Add("2.手勢鎖");
            labels.Add("3.創建OR恢復");
            labels.Add("4.錢包命名");
            labels.Add("錢包恢復");
            labels.Add("錢包創建完成");
            labels.Add("如您是首次使用數位錢包，請選擇該項");
            labels.Add("如您曾通過本產品創建數位錢包，可選擇此項進行助記詞恢復");
            labels.Add("請輸入助記詞");
            labels.Add("輸入時，請在每兩個詞之間加上空格分隔");
            labels.Add("請勿拍攝並確保無人觀看");
            labels.Add("助記詞用於恢復錢包和重置錢包密碼，請一定要抄錄下來並妥善保管好。");
            labels.Add("今後不再顯示");
            labels.Add("Hi，歡迎使用dimension\r\n大師數位錢包");
            labels.Add("我們將為您提供安全與簡單的數字資產保護方案。");
            labels.Add("開始設置");
            labels.Add("錢包未備份");
            labels.Add("請為您的錢包設置\r\n由4位元數字組成的名稱");
            labels.Add("創建成功！");
            labels.Add("請使用dimension APP配對後使用");
            labels.Add("         接收錢款");
            labels.Add("         付款簽名");
            labels.Add("         設備配對");
            labels.Add("秒");
            labels.Add("30秒");
            labels.Add("1分鐘");
            labels.Add("2分鐘");
            labels.Add("5分鐘");
            labels.Add("永不");
            labels.Add("掃二維碼完成簽名");
            labels.Add("簽名信息");
            labels.Add("幣種");
            labels.Add("礦工費");
            labels.Add("數量");
            labels.Add("接收位址");
            labels.Add("發送地址");
            labels.Add("確認簽名後，請打開熱錢包掃描簽名後的二維碼");
            labels.Add("確認簽名");
            labels.Add("取  消");
            labels.Add("簽名支付");
            labels.Add("請在手機上打開dimension掃一掃");
            labels.Add("上一張");
            labels.Add("下一張");
            labels.Add("輸入密碼");
            labels.Add("錯誤");
            labels.Add("掃描完成\r\n");
            labels.Add("請掃描下一張二維碼");
            labels.Add("此二維碼已掃過，請掃描下一張二維碼");
            labels.Add("提示");
            labels.Add("創建中...");
            labels.Add("關機");
            labels.Add("重啟");
        }
    }

    public class English : Language
    {
        public English()
        {
            labels = new List<string>();
            labels.Add("Set password");
            labels.Add("Next");
            labels.Add("Confirm password");
            labels.Add("Confirm");
            labels.Add("Set gesture lock");
            labels.Add("At least four points passed");
            labels.Add("Confirm gesture lock");
            labels.Add("Create and restore");
            labels.Add("Create wallet");
            labels.Add("Restore wallet");
            labels.Add("Create new currency");
            labels.Add("Device pairing");
            labels.Add("Beneficiary's address");
            labels.Add("Signature");
            labels.Add("Signed transaction");
            labels.Add("Please open \r\ndimension on the mobile phone to scan");
            labels.Add("Currency management");
            labels.Add("Modify gesture lock");
            labels.Add("Lock screen clock");
            labels.Add("Wallet backup");
            labels.Add("Language ");
            labels.Add("Brightness");
            labels.Add("Factory settings");
            labels.Add("Explanation for the backup phrase");
            labels.Add("Setting");
            labels.Add("I know");
            labels.Add("Return");
            labels.Add("I have copied it");
            labels.Add("Please remember the following backup phrase");
            labels.Add("Confirm the backup  phrase");
            labels.Add("The backup phrase is used to restore and reset the password of the wallet, so please make sure that you copy it and keep it properly");
            labels.Add("Scan QR Code to complete signature");
            labels.Add("Incorrect password");
            labels.Add("Please input an 8-digit password");
            labels.Add("Two different passwords input");
            labels.Add("Two different gestures");
            labels.Add("Internal errors");
            labels.Add("Error in the backup phrase");
            labels.Add("Factory settings");
            labels.Add("Current gesture lock");
            labels.Add("Set the new gesture lock");
            labels.Add("Confirm the new gesture lock");
            labels.Add("Succeed in modifying the gesture lock");
            labels.Add("Unlock gesture");
            labels.Add("Equipment downtime, remaining time: {0} second");
            labels.Add("Unrecognized QR Code");
            labels.Add("Repeat password");
            labels.Add("Please repeat 8-digit password");
            labels.Add("Please enter gesture lock");
            labels.Add("Please repeat gesture lock ");
            labels.Add("dimension");
            labels.Add("Welcome");
            labels.Add("1. Set password");
            labels.Add("2. Gesture lock ");
            labels.Add("3. Create OR restore");
            labels.Add("4. Name wallet");
            labels.Add(" Restore wallet");
            labels.Add("Creation of the wallet completed");
            labels.Add("If you are using a digital wallet for the first time, please choose this item");
            labels.Add("If you have created a digital wallet via this product, choose this item to restore the backup phrase");
            labels.Add("Please enter the backup phrase");
            labels.Add("Please add a space for separation between every two words when entering");
            labels.Add("Do not take a picture and make sure that no one is watching");
            labels.Add("The backup phrase is to recover wallet and reset its password, please make a copy of it and keep it properly. ");
            labels.Add("Do not show again");
            labels.Add("Hi,welcome to use dimension\r\n Digital Wallet");
            labels.Add("We will offer you safe and simple digital asset protection schemes");
            labels.Add("Start setting");
            labels.Add("Wallet not backuped");
            labels.Add("Please set \r\n name consisting of 4 digit numbers");
            labels.Add("Created successfully! ");
            labels.Add("Please use it after pairing it up with dimension APP");
            labels.Add("         Receive payment");
            labels.Add("         Payment signature");
            labels.Add("         Device pairing");
            labels.Add("Second");
            labels.Add("30 seconds");
            labels.Add("1 minute");
            labels.Add("2 minutes");
            labels.Add("5 minutes");
            labels.Add("Never");
            labels.Add("Scan QR Code to complete the signature");
            labels.Add("Signature information");
            labels.Add("Currency");
            labels.Add("Gas fee");
            labels.Add("Quantity");
            labels.Add("Receiving address");
            labels.Add("Sending address");
            labels.Add("After confirming the signature, please open dimension Wallet and scan the signed QR Code");
            labels.Add("Confirm signature");
            labels.Add("Cancel");
            labels.Add("Signature payment");
            labels.Add("Please open DarcoWallet and scan QR Code");
            labels.Add("Previous ");
            labels.Add("Next ");
            labels.Add("Enter password");
            labels.Add("Error");
            labels.Add("Scan completed\r\n");
            labels.Add("Please scan the next QR Code");
            labels.Add("This QR Code has been scanned. Please scan the next one");
            labels.Add("Tips");
            labels.Add("Creating...");
            labels.Add("Power off");
            labels.Add("Restart");
        }
    }

    public class Janpanese : Language
    {
        public Janpanese()
        {
            labels = new List<string>();
            labels.Add("パスワード設定");
            labels.Add("次へ");
            labels.Add("パスワード確認");
            labels.Add("確  認");
            labels.Add("ジェスチャーロック設定");
            labels.Add("なくとも4つの点をつないでください。");
            labels.Add("ジェスチャーロック確認");
            labels.Add("新規とリカバー");
            labels.Add("新規ℯ-財布");
            labels.Add("ℯ-財布をリカバーする");
            labels.Add("新規通貨");
            labels.Add("端末マッチング");
            labels.Add("振込先");
            labels.Add("サイン");
            labels.Add("サイン取引");
            labels.Add("QRコードをスキャンしてください。");
            labels.Add("通貨管理");
            labels.Add("ジェスチャーロック変更");
            labels.Add("ロック画面時間");
            labels.Add("ℯ-財布バックアップ");
            labels.Add("言語切替");
            labels.Add("光度設定");
            labels.Add("初期化");
            labels.Add("ヒントワードの説明");
            labels.Add("設定");
            labels.Add("分かりました");
            labels.Add("戻る");
            labels.Add("記録を取りました");
            labels.Add("次のヒントワードを覚えてください");
            labels.Add("ヒントワード確認");
            labels.Add("ヒントワードはリカバリーとリセットに必要であるため、必ずメモして、大切に保管してください。");
            labels.Add("QRコードをスキャンしてサインが終了です。");
            labels.Add("パスワードエラー");
            labels.Add("８桁のパスワードを入力してください。");
            labels.Add("パスワードが一致しません。");
            labels.Add("ジェスチャーロックが一致しません。");
            labels.Add("システムエラー");
            labels.Add("ヒントワードエラー");
            labels.Add("初期化");
            labels.Add("使用中のジェスチャーロック");
            labels.Add("新規ジェスチャーロック");
            labels.Add("新しいジェスチャーロックを確認する");
            labels.Add("ジェスチャーロック変更完了");
            labels.Add("ジェスチャーロックを解除する");
            labels.Add("端末停止　残り時間：　秒");
            labels.Add("QRコード識別不能");
            labels.Add("もう一回入力してください");
            labels.Add("８桁のパスワードを入力してください。");
            labels.Add("ジェスチャーロックを解除してください。");
            labels.Add("ジェスチャーを繰り返してください。");
            labels.Add("dimension");
            labels.Add("いらっしゃいませ");
            labels.Add("1.パスワード設定");
            labels.Add("2.ジェスチャーロック");
            labels.Add("3.新規ORリカバー");
            labels.Add("4.アカウント名設定");
            labels.Add("ℯ-財布をリカバーする");
            labels.Add("ℯ-財布新規完了");
            labels.Add("初めてご利用になる方はこちらを選んでください");
            labels.Add("ℯ-財布を新規したいと存じる方は、ヒントワードをお選びください。");
            labels.Add("ヒントワードを入力してください。");
            labels.Add("二つの単語の間に1字空けてください。");
            labels.Add("写真を撮ってはいけません。周囲の不審者をご注意ください。");
            labels.Add("ヒントワードはリカバリーとリセットに必要であるため、必ずメモして、大切に保管してください。");
            labels.Add("これから表示が中止されます。");
            labels.Add("HI！dimension大師ℯ-財布をご利用いただき、ありがとうございます。");
            labels.Add("セフティーでイージーな電子資産保護プランを提供いたします。");
            labels.Add("新規作成");
            labels.Add("ℯ-財布はバックアップしていません。");
            labels.Add("4桁の数字でアカウント名をセットしてください。");
            labels.Add("セット完了");
            labels.Add("端末をこのアプリとマッチングしてください。");
            labels.Add("         入金");
            labels.Add("         支払いサイン");
            labels.Add("         端末マッチング");
            labels.Add("秒");
            labels.Add("30秒");
            labels.Add("1分");
            labels.Add("2分");
            labels.Add("5分");
            labels.Add("ネバー");
            labels.Add("QRコードをスキャンしてサインを完成してください。");
            labels.Add("サイン情報");
            labels.Add("通貨種類");
            labels.Add("滞納金");
            labels.Add("数量");
            labels.Add("入金先");
            labels.Add("出金先");
            labels.Add("サインを確認してから、QRコードをスキャンしてください。");
            labels.Add("サイン確認");
            labels.Add("キャンセル");
            labels.Add("サイン支払い");
            labels.Add("スキャンをご用意してください。");
            labels.Add("前へ");
            labels.Add("次へ");
            labels.Add("パスワードをご入力してください");
            labels.Add("エラー");
            labels.Add("スキャン完了");
            labels.Add("次のQRコードをスキャンしてください。");
            labels.Add("このQRコードはスキャン済みだったので次のをスキャンしてください。");
            labels.Add("提示");
            labels.Add("作成中…");
            labels.Add("関機");
            labels.Add("再起動");
        }
    }
}
