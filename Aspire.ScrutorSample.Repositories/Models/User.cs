using System.ComponentModel.DataAnnotations.Schema;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Aspire.ScrutorSample.Repositories.Models;

[Table("user")]
public class User
{
	/// <summary>
	/// 識別碼
	/// </summary>
	[BsonId]
	[BsonElement("id")]
	[BsonRepresentation(BsonType.String)]
	public string Id { get; set; } = null!;

	/// <summary>
	/// 用戶名
	/// </summary>
	[BsonElement("username")]
	[BsonRepresentation(BsonType.String)]
	public string Username { get; set; } = null!;

	/// <summary>
	/// 密碼
	/// </summary>
	[BsonElement("password")]
	[BsonRepresentation(BsonType.String)]
	public string Password { get; set; } = null!;

	/// <summary>
	/// 狀態
	/// </summary>
	[BsonElement("state")]
	[BsonRepresentation(BsonType.Int32)]
	public int State { get; set; }

	/// <summary>
	/// 創建時間
	/// </summary>
	[BsonElement("created_at")]
	[BsonRepresentation(BsonType.DateTime)]
	public DateTimeOffset CreatedAt { get; set; }

	/// <summary>
	/// 更新時間
	/// </summary>
	[BsonElement("update_at")]
	[BsonRepresentation(BsonType.DateTime)]
	public DateTimeOffset UpdateAt { get; set; }
}
