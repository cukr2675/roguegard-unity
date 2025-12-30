# 設計メモ

- / フォルダ構成
    - IListuiManager.cs: List-UI の基本となるマネージャーインターフェース
    - IListuiArg.cs: List-UI で各画面の引数となるインターフェース
    - /Handlers: ビューと ViewData の対話方法を提供する
    - /Screens: List-UI の画面単位である IListuiScreen<,> とそのナビゲーション方法を提供する
        - /ViewData: 画面単位のビューを制御する ViewData クラス
    - /ViewInterfaces: ビューと ViewData の接続点を提供する

- ViewData は /Handlers と /ViewInterfaces を使ってビューと対話し、\
  その ViewData を IListuiScreen<,> でラップして画面ナビゲーションを管理するイメージ

- /ViewObject フォルダ構成
    - /CommonPrefabs: Lysionium 全般で使用する Prefab
    - /Subviews: ViewData と対話するビューの実装と Prefab
    - /ViewItems, /ViewWidgets: ビューに表示する項目の実装と Prefab
    
- フォルダ依存関係
    - /Managers は /ViewObject に依存する
    - /ViewObject は /Managers 以外の Lysionium アセンブリに依存する
    - 循環依存は禁止


# List-UI

- リストデータとハンドラでメニュー画面を生成しようという考え方が従来の List-menu
- List-menu を拡張して、仮想パッドや HUD も生成しようという考え方が List-UI

## List-UI 使用時の推奨設計

- 基本フォルダ (以下 /Listui) 直下に IListuiManager (必要なら IListuiArg も) の実装クラスを配置
- /Listui/Handlers に IViewItemHandler の実装クラスや派生インターフェースを配置
- /Listui/Screens に List-UI 画面クラスを配置
- /Listui/Screens/ViewData に ViewData クラスを配置
- /Listui/ViewInterfaces にビューのインターフェースを配置
