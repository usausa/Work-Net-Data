﻿namespace DataLibrary.Generator
{
    using DataLibrary.Engine;

    public class DaoGenerator
    {
        // コンパイルタイムを考えると、生成コード中にConfigを参照するコードがあってはだめ

        // TODO 予約変数 _con, _tx, _timeout? sql cursorOption?, cancel

        // TODO パラメータは1インスタンスのIEvalにして、複数の時はAggregateEval(Array, List)を指定する形にするか？

        // TODO converterはDBに特化した最適版、Mapperでも？、違う、Tは無限か！、でも独自にして登録もあり！
        // いや、ソースもパターンがあるから無理か…

        // TODO DBパラメータの最適化？

        public DaoGenerator(DaoGeneratorOption option, IComponentResolver resolver)
        {
        }
    }
}
