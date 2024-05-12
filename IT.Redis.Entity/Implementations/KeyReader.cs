namespace IT.Redis.Entity;

public delegate byte[] KeyReader<TEntity>(TEntity entity, IKeyRebuilder keyBuilder);