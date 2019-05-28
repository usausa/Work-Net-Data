namespace DataLibrary.Generator
{
    using DataLibrary.Engine;

    public class DaoGenerator
    {
        // コンパイルタイムを考えると、生成コード中にConfigを参照するコードがあってはだめ

        // TODO 予約変数 _con, _tx, _timeout? sql cursorOption?, cancel

        // TODO パラメータは1インスタンスのIEvalにして、複数の時はAggregateEval(Array, List)を指定する形にするか？

        // TODO converterはDBに特化した最適版、Mapperでも？、違う、Tは無限か！、でも独自にして登録もあり！
        // いや、ソースもパターンがあるから無理か…
        // TODO but ExecuteScalarのTを限定することで、問題なくなる？ でもDateTimeとかは処理したい
        // TODO 特定型のみで、ルールが入らないならConvertとどちがら早いか検証
        // TODO 属性で処理する？TypeHandler

        // TODO DBパラメータの最適化？

        // TODO CMDの扱いも呼び出し側にする

        // TODO SQLパラメータにローカル変数予約方式があったら例外「__にする？」

        // TODO パラメータは連番方式か？ @:はそのまま？ Providerで指定でも良いが

        // TODO Array,IEは自動展開？、stringは除外？

        // TODO フラット化？、特定型以外？、明示的フラット/非フラット属性？
        // Expreはもとにたいしてやる必要があるから、とりあえずフラットはなし？

        public DaoGenerator(DaoGeneratorOption option, IComponentResolver resolver)
        {
        }
    }
}
