# 174g-Shaders

VDMXで使うISFシェーダーのコレクションです。シェーダーは `shaders/` フォルダに順次追加します。

ファイル名の先頭に付く `17` はinaharu、`4g` は志賀、`174g` は二人による共同制作を示します。

## シェーダー

- [17-Resize Glitch.fs](shaders/17-Resize%20Glitch.fs) — VIDVOXのResize Glitchをベースに、グリッチの発生頻度、表示時間、変化速度、倍率などを調整できるエフェクト。Frequencyは1秒あたりの発生回数（Hz）、Durationは1周期に対する表示時間の割合（0〜1）です。
- [4g-Stamp Trail.fs](shaders/4g-Stamp%20Trail.fs) — 軽量な持続型画像履歴（スタンプトレイル）エフェクト。時間経過による消失ではなく、Trail Amount（0〜100%）で履歴の保持・消去を調整します。
- [17-Strobe.fs](shaders/17-Strobe.fs) — 白→黒→元映像、色反転、白の3モードを切り替えられるストロボ。Frequencyで速度、Intensityで強さを調整します。白→黒→元映像は各状態を周期の3分の1ずつ、ほかのモードは効果と元映像を半分ずつ表示します。

## 使い方

`.fs` ファイルをVDMXのISFエフェクトとして読み込み、映像に適用します。各パラメーターはVDMX上で調整できます。

Changes per Secondが0のときは、グリッチ発生中はサイズと位置を固定します。値を上げると、発生中にもサイズや位置が変化します。
初期設定では4秒ごとにサイズが滑らかに変わり、次の変化までそのサイズを保ちます。Durationを1未満にすると、周期の残りは元の映像に戻ります。

## クレジット

Resize GlitchはVIDVOXのResize Glitchをベースにしています。元のシェーダーのクレジットはファイル内にも記載しています。

元の作品のライセンスは [VIDVOX-LICENSE.txt](VIDVOX-LICENSE.txt) を参照してください（出典：[VIDVOX ISF-Files](https://github.com/Vidvox/ISF-Files)）。
