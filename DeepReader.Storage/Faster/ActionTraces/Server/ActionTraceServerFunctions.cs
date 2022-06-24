using DeepReader.Storage.Faster.ActionTraces.Base;
using DeepReader.Storage.Faster.ActionTraces.Standalone;
using DeepReader.Types.StorageTypes;
using FASTER.core;

namespace DeepReader.Storage.Faster.ActionTraces.Server;

public sealed class ActionTraceServerFunctions : IFunctions<ulong, ActionTrace, ActionTraceInput, ActionTraceOutput, long>
{
    // @Haron
    // not sure which Methods here really need a body. I just now the ConcurrentReader needs one in ActionTraceFunctions

    /// <summary>
    /// Non-concurrent reader. 
    /// </summary>
    /// <param name="key">The key for the record to be read</param>
    /// <param name="input">The user input for computing <paramref name="dst"/> from <paramref name="value"/></param>
    /// <param name="value">The value for the record being read</param>
    /// <param name="dst">The location where <paramref name="value"/> is to be copied</param>
    /// <param name="readInfo">Information about this read operation and its context</param>
    /// <returns>True if the value was available, else false (e.g. the value was expired)</returns>
    public bool SingleReader(ref ulong key, ref ActionTraceInput input, ref ActionTrace value, ref ActionTraceOutput dst,
        ref ReadInfo readInfo)
    {

        return true;
    }

    /// <summary>
    /// Conncurrent reader
    /// </summary>
    /// <param name="key">The key for the record to be read</param>
    /// <param name="input">The user input for computing <paramref name="dst"/> from <paramref name="value"/></param>
    /// <param name="value">The value for the record being read</param>
    /// <param name="dst">The location where <paramref name="value"/> is to be copied</param>
    /// <param name="readInfo">Information about this read operation and its context</param>
    /// <returns>True if the value was available, else false (e.g. the value was expired)</returns>
    public bool ConcurrentReader(ref ulong key, ref ActionTraceInput input, ref ActionTrace value, ref ActionTraceOutput dst,
        ref ReadInfo readInfo)
    {
        dst.Value = value;
        return true;
    }

    /// <summary>
    /// Read completion
    /// </summary>
    /// <param name="key">The key for this record</param>
    /// <param name="input">The user input that was used in the read operation</param>
    /// <param name="output">The result of the read operation; if this is a struct, then it will be a temporary and should be copied to <paramref name="ctx"/></param>
    /// <param name="ctx">The application context passed through the pending operation</param>
    /// <param name="status">The result of the pending operation</param>
    /// <param name="recordMetadata">Metadata for the record; may be used to obtain <see cref="RecordMetadata.RecordInfo"/>.PreviousAddress when doing iterative reads</param>
    public void ReadCompletionCallback(ref ulong key, ref ActionTraceInput input, ref ActionTraceOutput output, long ctx,
        Status status, RecordMetadata recordMetadata)
    {

    }

    /// <summary>
    /// Non-concurrent writer; called on an Upsert that does not find the key so does an insert or finds the key's record in the immutable region so does a read/copy/update (RCU).
    /// </summary>
    /// <param name="key">The key for this record</param>
    /// <param name="input">The user input to be used for computing <paramref name="dst"/></param>
    /// <param name="src">The previous value to be copied/updated</param>
    /// <param name="dst">The destination to be updated; because this is an copy to a new location, there is no previous value there.</param>
    /// <param name="output">The location where the result of the update may be placed</param>
    /// <param name="upsertInfo">Information about this update operation and its context</param>
    /// <param name="reason">The operation for which this write is being done</param>
    /// <returns>True if the write was performed, else false (e.g. cancellation)</returns>
    public bool SingleWriter(ref ulong key, ref ActionTraceInput input, ref ActionTrace src, ref ActionTrace dst,
        ref ActionTraceOutput output, ref UpsertInfo upsertInfo, WriteReason reason)
    {
        return true;
    }

    /// <summary>
    /// Called after SingleWriter when a record containing an upsert of a new key has been successfully inserted at the tail of the log.
    /// </summary>
    /// <param name="key">The key for this record</param>
    /// <param name="input">The user input that was used to compute <paramref name="dst"/></param>
    /// <param name="src">The previous value to be copied/updated</param>
    /// <param name="dst">The destination to be updated; because this is an copy to a new location, there is no previous value there.</param>
    /// <param name="output">The location where the result of the update may be placed</param>
    /// <param name="upsertInfo">Information about this update operation and its context</param>
    /// <param name="reason">The operation for which this write is being done</param>
    public void PostSingleWriter(ref ulong key, ref ActionTraceInput input, ref ActionTrace src, ref ActionTrace dst,
        ref ActionTraceOutput output, ref UpsertInfo upsertInfo, WriteReason reason)
    {

    }

    /// <summary>
    /// Concurrent writer; called on an Upsert that finds the record in the mutable range.
    /// </summary>
    /// <param name="key">The key for the record to be written</param>
    /// <param name="input">The user input to be used for computing <paramref name="dst"/></param>
    /// <param name="src">The value to be copied to <paramref name="dst"/></param>
    /// <param name="dst">The location where <paramref name="src"/> is to be copied; because this method is called only for in-place updates, there is a previous value there.</param>
    /// <param name="output">The location where the result of the update may be placed</param>
    /// <param name="upsertInfo">Information about this update operation and its context</param>
    /// <returns>True if the value was written, else false</returns>
    public bool ConcurrentWriter(ref ulong key, ref ActionTraceInput input, ref ActionTrace src, ref ActionTrace dst,
        ref ActionTraceOutput output, ref UpsertInfo upsertInfo)
    {
        return true;
    }

    /// <summary>
    /// Whether we need to invoke initial-update for RMW
    /// </summary>
    /// <param name="key">The key for this record</param>
    /// <param name="input">The user input to be used for computing the updated value</param>
    /// <param name="output">The location where the result of the <paramref name="input"/> operation is to be copied</param>
    /// <param name="rmwInfo">Information about this update operation and its context</param>
    public bool NeedInitialUpdate(ref ulong key, ref ActionTraceInput input, ref ActionTraceOutput output, ref RMWInfo rmwInfo)
    {
        return false;
    }

    /// <summary>
    /// Initial update for RMW (insert at the tail of the log).
    /// </summary>
    /// <param name="key">The key for this record</param>
    /// <param name="input">The user input to be used for computing the updated <paramref name="value"/></param>
    /// <param name="value">The destination to be updated; because this is an insert, there is no previous value there.</param>
    /// <param name="output">The location where the result of the <paramref name="input"/> operation on <paramref name="value"/> is to be copied</param>
    /// <param name="rmwInfo">Information about this update operation and its context</param>
    /// <returns>True if the write was performed, else false (e.g. cancellation)</returns>
    public bool InitialUpdater(ref ulong key, ref ActionTraceInput input, ref ActionTrace value, ref ActionTraceOutput output,
        ref RMWInfo rmwInfo)
    {
        return true;
    }

    /// <summary>
    /// Called after a record containing an initial update for RMW has been successfully inserted at the tail of the log.
    /// </summary>
    /// <param name="key">The key for this record</param>
    /// <param name="input">The user input to be used for computing the updated <paramref name="value"/></param>
    /// <param name="value">The destination to be updated; because this is an insert, there is no previous value there.</param>
    /// <param name="output">The location where the result of the <paramref name="input"/> operation on <paramref name="value"/> is to be copied</param>
    /// <param name="rmwInfo">Information about this update operation and its context</param>
    public void PostInitialUpdater(ref ulong key, ref ActionTraceInput input, ref ActionTrace value, ref ActionTraceOutput output,
        ref RMWInfo rmwInfo)
    {

    }

    /// <summary>
    /// Whether we need to invoke copy-update for RMW
    /// </summary>
    /// <param name="key">The key for this record</param>
    /// <param name="input">The user input to be used for computing the updated value</param>
    /// <param name="oldValue">The existing value that would be copied.</param>
    /// <param name="output">The location where the result of the <paramref name="input"/> operation on <paramref name="oldValue"/> is to be copied</param>
    /// <param name="rmwInfo">Information about this update operation and its context</param>
    public bool NeedCopyUpdate(ref ulong key, ref ActionTraceInput input, ref ActionTrace oldValue, ref ActionTraceOutput output,
        ref RMWInfo rmwInfo)
    {
        return false;
    }

    /// <summary>
    /// Copy-update for RMW (RCU (Read-Copy-Update) to the tail of the log)
    /// </summary>
    /// <param name="key">The key for this record</param>
    /// <param name="input">The user input to be used for computing <paramref name="newValue"/> from <paramref name="oldValue"/></param>
    /// <param name="oldValue">The previous value to be copied/updated</param>
    /// <param name="newValue">The destination to be updated; because this is an copy to a new location, there is no previous value there.</param>
    /// <param name="output">The location where <paramref name="newValue"/> is to be copied</param>
    /// <param name="rmwInfo">Information about this update operation and its context</param>
    /// <returns>True if the write was performed, else false (e.g. cancellation)</returns>
    public bool CopyUpdater(ref ulong key, ref ActionTraceInput input, ref ActionTrace oldValue, ref ActionTrace newValue,
        ref ActionTraceOutput output, ref RMWInfo rmwInfo)
    {
        return true;
    }

    /// <summary>
    /// Called after a record containing an RCU (Read-Copy-Update) for RMW has been successfully inserted at the tail of the log.
    /// </summary>
    /// <param name="key">The key for this record</param>
    /// <param name="input">The user input to be used for computing <paramref name="newValue"/> from <paramref name="oldValue"/></param>
    /// <param name="oldValue">The previous value to be copied/updated; may also be disposed here if appropriate</param>
    /// <param name="newValue">The destination to be updated; because this is an copy to a new location, there is no previous value there.</param>
    /// <param name="output">The location where <paramref name="newValue"/> is to be copied</param>
    /// <param name="rmwInfo">Information about this update operation and its context</param>
    public void PostCopyUpdater(ref ulong key, ref ActionTraceInput input, ref ActionTrace oldValue, ref ActionTrace newValue,
        ref ActionTraceOutput output, ref RMWInfo rmwInfo)
    {

    }

    /// <summary>
    /// In-place update for RMW
    /// </summary>
    /// <param name="key">The key for this record</param>
    /// <param name="input">The user input to be used for computing the updated <paramref name="value"/></param>
    /// <param name="value">The destination to be updated; because this is an in-place update, there is a previous value there.</param>
    /// <param name="output">The location where the result of the <paramref name="input"/> operation on <paramref name="value"/> is to be copied</param>
    /// <param name="rmwInfo">Information about this update operation and its context</param>
    /// <returns>True if the value was successfully updated, else false (e.g. the value was expired)</returns>
    public bool InPlaceUpdater(ref ulong key, ref ActionTraceInput input, ref ActionTrace value, ref ActionTraceOutput output,
        ref RMWInfo rmwInfo)
    {
        return true;
    }

    /// <summary>
    /// RMW completion
    /// </summary>
    /// <param name="key">The key for this record</param>
    /// <param name="input">The user input that was used to perform the modification</param>
    /// <param name="output">The result of the RMW operation; if this is a struct, then it will be a temporary and should be copied to <paramref name="ctx"/></param>
    /// <param name="ctx">The application context passed through the pending operation</param>
    /// <param name="status">The result of the pending operation</param>
    /// <param name="recordMetadata">The metadata of the modified or inserted record</param>
    public void RMWCompletionCallback(ref ulong key, ref ActionTraceInput input, ref ActionTraceOutput output, long ctx,
        Status status, RecordMetadata recordMetadata)
    {

    }

    /// <summary>
    /// Single deleter; called on a Delete that does not find the record in the mutable range and so inserts a new record.
    /// </summary>
    /// <param name="key">The key for the record to be deleted</param>
    /// <param name="value">The value for the record being deleted; because this method is called only for in-place updates, there is a previous value there. Usually this is ignored or assigned 'default'.</param>
    /// <param name="deleteInfo">Information about this update operation and its context</param>
    /// <remarks>For Object Value types, Dispose() can be called here. If recordInfo.Invalid is true, this is called after the record was allocated and populated, but could not be appended at the end of the log.</remarks>
    /// <returns>True if the value was successfully deleted, else false (e.g. the record was sealed)</returns>
    /// <returns>True if the deleted record should be added, else false (e.g. cancellation)</returns>
    public bool SingleDeleter(ref ulong key, ref ActionTrace value, ref DeleteInfo deleteInfo)
    {
        return true;
    }

    /// <summary>
    /// Called after a record marking a Delete (with Tombstone set) has been successfully inserted at the tail of the log.
    /// </summary>
    /// <param name="key">The key for the record that was deleted</param>
    /// <param name="deleteInfo">Information about this update operation and its context</param>
    /// 
    /// <remarks>This does not have the address of the record that contains the value at 'key'; Delete does not retrieve records below HeadAddress, so
    ///     the last record we have in the 'key' chain may belong to 'key' or may be a collision.</remarks>
    public void PostSingleDeleter(ref ulong key, ref DeleteInfo deleteInfo)
    {

    }

    /// <summary>
    /// Concurrent deleter; called on a Delete that finds the record in the mutable range.
    /// </summary>
    /// <param name="key">The key for the record to be deleted</param>
    /// <param name="value">The value for the record being deleted; because this method is called only for in-place updates, there is a previous value there. Usually this is ignored or assigned 'default'.</param>
    /// <param name="deleteInfo">Information about this update operation and its context</param>
    /// <remarks>For Object Value types, Dispose() can be called here. If recordInfo.Invalid is true, this is called after the record was allocated and populated, but could not be appended at the end of the log.</remarks>
    /// <returns>True if the value was successfully deleted, else false (e.g. the record was sealed)</returns>
    public bool ConcurrentDeleter(ref ulong key, ref ActionTrace value, ref DeleteInfo deleteInfo)
    {
        return true;
    }

    /// <summary>
    /// Called after SingleWriter, if the CAS insertion of record into the store fails. Can be used to perform object disposal related actions.
    /// </summary>
    /// <param name="key">The key for this record</param>
    /// <param name="input">The user input that was used to compute <paramref name="dst"/></param>
    /// <param name="src">The previous value to be copied/updated</param>
    /// <param name="dst">The destination to be updated; because this is an copy to a new location, there is no previous value there.</param>
    /// <param name="output">The location where the result of the update may be placed</param>
    /// <param name="upsertInfo">Information about this update operation and its context</param>
    /// <param name="reason">The operation for which this write is being done</param>
    public void DisposeSingleWriter(ref ulong key, ref ActionTraceInput input, ref ActionTrace src, ref ActionTrace dst,
        ref ActionTraceOutput output, ref UpsertInfo upsertInfo, WriteReason reason)
    {

    }

    /// <summary>
    /// Called after copy-update for RMW (RCU (Read-Copy-Update) to the tail of the log), if the CAS insertion of record into the store fails. Can be used to perform object disposal related actions.
    /// </summary>
    /// <param name="key">The key for this record</param>
    /// <param name="input">The user input to be used for computing <paramref name="newValue"/> from <paramref name="oldValue"/></param>
    /// <param name="oldValue">The previous value to be copied/updated</param>
    /// <param name="newValue">The destination to be updated; because this is an copy to a new location, there is no previous value there.</param>
    /// <param name="output">The location where <paramref name="newValue"/> is to be copied</param>
    /// <param name="rmwInfo">Information about this update operation and its context</param>
    public void DisposeCopyUpdater(ref ulong key, ref ActionTraceInput input, ref ActionTrace oldValue, ref ActionTrace newValue,
        ref ActionTraceOutput output, ref RMWInfo rmwInfo)
    {

    }

    /// <summary>
    /// Called after initial update for RMW (insert at the tail of the log), if the CAS insertion of record into the store fails. Can be used to perform object disposal related actions.
    /// </summary>
    /// <param name="key">The key for this record</param>
    /// <param name="input">The user input to be used for computing the updated <paramref name="value"/></param>
    /// <param name="value">The destination to be updated; because this is an insert, there is no previous value there.</param>
    /// <param name="output">The location where the result of the <paramref name="input"/> operation on <paramref name="value"/> is to be copied</param>
    /// <param name="rmwInfo">Information about this update operation and its context</param>
    public void DisposeInitialUpdater(ref ulong key, ref ActionTraceInput input, ref ActionTrace value,
        ref ActionTraceOutput output, ref RMWInfo rmwInfo)
    {

    }

    /// <summary>
    /// Called after a Delete that does not find the record in the mutable range and so inserts a new record, if the CAS insertion of record into the store fails. Can be used to perform object disposal related actions.
    /// </summary>
    /// <param name="key">The key for the record to be deleted</param>
    /// <param name="value">The value for the record being deleted; because this method is called only for in-place updates, there is a previous value there. Usually this is ignored or assigned 'default'.</param>
    /// <param name="deleteInfo">Information about this update operation and its context</param>
    /// <remarks>For Object Value types, Dispose() can be called here. If recordInfo.Invalid is true, this is called after the record was allocated and populated, but could not be appended at the end of the log.</remarks>
    public void DisposeSingleDeleter(ref ulong key, ref ActionTrace value, ref DeleteInfo deleteInfo)
    {

    }

    /// <summary>
    /// Called after a record has been deserialized from the disk on a pending Read or RMW. Can be used to perform object disposal related actions.
    /// </summary>
    /// <param name="key">The key for the record</param>
    /// <param name="value">The value for the record</param>
    public void DisposeDeserializedFromDisk(ref ulong key, ref ActionTrace value)
    {

    }

    /// <summary>
    /// Checkpoint completion callback (called per client session)
    /// </summary>
    /// <param name="sessionID">ID of session reporting persistence</param>
    /// <param name="sessionName">Name of session reporting persistence</param>
    /// <param name="commitPoint">Commit point descriptor</param>
    public void CheckpointCompletionCallback(int sessionID, string sessionName, CommitPoint commitPoint)
    {

    }
}