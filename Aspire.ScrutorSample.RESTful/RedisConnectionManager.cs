using StackExchange.Redis;

namespace Aspire.ScrutorSample.RESTful;

internal class RedisConnectionManager(IConnectionMultiplexer multiplexer) : IDisposable
{
	private readonly IConnectionMultiplexer _connectionMultiplexer = multiplexer;
	private bool _disposedValue;

	public IDatabase Database => _connectionMultiplexer.GetDatabase();

	public void Dispose()
	{
		// 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				_connectionMultiplexer.Close();
				_connectionMultiplexer.Dispose();
			}

			_disposedValue = true;
		}
	}
}
