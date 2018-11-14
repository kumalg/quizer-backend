using System;

namespace quizer_backend.Data.Services {
    public class BaseService {
        protected QuizerContext Context;
        protected long CurrentTime => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        public BaseService(QuizerContext context) {
            Context = context;
        }
    }
}
