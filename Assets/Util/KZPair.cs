public class KZPair<T, K> { 
    private readonly T _lhs;
    private readonly K _rhs;

    //convenient getters
    public T a {
        get { return _lhs; }
    }
    public K b {
        get { return _rhs; }
    }
    public T left {
        get { return _lhs; }
    }
    public K right {
        get { return _rhs; }
    }
    public T lhs {
        get { return _lhs; }
    }
    public K rhs {
        get { return _rhs; }
    }

    public KZPair(T _lhs, K _rhs) {
        this._lhs=_lhs;
        this._rhs=_rhs;
    }

    public static void test() {
        //KZPair<int, float> t=new KZPair<int, float>(3, 3.14f);
        //int a=t.a;
        //float b=t.b;
    }
}
