﻿namespace quizer_backend.Data.Entities {
    public abstract class BaseEntity { }

    public abstract class Entity<T> : BaseEntity, IEntity<T> {
        public virtual T Id { get; set; }
    }
}
