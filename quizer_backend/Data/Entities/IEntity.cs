namespace quizer_backend.Data.Entities {
    public interface IEntity<T> {
        T Id { get; set; }
    }
}
