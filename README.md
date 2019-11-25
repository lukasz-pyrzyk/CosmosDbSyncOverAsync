# Reproducing a thread hang when calling CosmosDB hang with sync-over-async 

## Setup

### 1. Create new environment variables. For testing purposed, I have used `West Europe` as a region

- - "CosmosdbHang-endpoint"
- - "CosmosdbHang-key"
- - "CosmosdbHang-databaseId"
- - "CosmosdbHang-containerId"
- - "CosmosdbHang-region"

## 3. Repro

1. Start application
2. Hit route `/heartbeat`
3. Request should finish successfully
4. Hit route again and wait for the response
5. If it was successful, go back to step 4. Usually it hangs on the 2 call, but I've spotted a situation when it did on 3rd
6. If you notice that the answer does not come, it means that the application has hanged