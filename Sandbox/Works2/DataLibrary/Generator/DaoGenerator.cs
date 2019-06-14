namespace DataLibrary.Generator
{
    using DataLibrary.Engine;

    public class DaoGenerator
    {
        // コンパイルタイムを考えると、生成コード中にConfigを参照するコードがあってはだめ

        // TODO (事前版)パラメータは1インスタンスのIEvalにして、複数の時はAggregateEval(Array, List)を指定する形にするか？
        // TODO パラメータは連番方式か？ @:はそのまま？ Providerで指定でも良いが
        // TODO フラット化？、特定型以外？、明示的フラット/非フラット属性？
        // Expressionはもとにたいしてやる必要があるから、とりあえずフラットはなし？

        public DaoGenerator(ExecuteEngine engine)
        {
        }
    }
}
