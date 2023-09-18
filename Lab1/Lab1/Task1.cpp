#include "Task1.h"

void printAdpaterInfo(PIP_ADAPTER_INFO pAdapter) {
    printf("\tDescription: \t\t%s\n", pAdapter->Description);
    printf("\tPhysical Address: \t");
    if (pAdapter->AddressLength == 0)
        printf("\n");
    for (UINT i = 0; i < pAdapter->AddressLength; i++) {
        if (i != (pAdapter->AddressLength - 1))
            printf("%.2X-", (int)pAdapter->Address[i]);
        else
            printf("%.2X\n", (int)pAdapter->Address[i]);
    }
    printf("\n");
}

void getMacAddress() {
    PIP_ADAPTER_INFO pAdapterInfo;

    ULONG outBufferLength = sizeof(IP_ADAPTER_INFO);
    pAdapterInfo = (IP_ADAPTER_INFO*)MALLOC(sizeof(IP_ADAPTER_INFO));
    if (pAdapterInfo == NULL) {
        printf("Error allocating memory for IP_ADAPTER_INFO struct\n");
        return;
    }

    // When buffer overflow error code is returned, the outBufferLength parameter contains the required buffer size.
    if (GetAdaptersInfo(pAdapterInfo, &outBufferLength) == ERROR_BUFFER_OVERFLOW) {
        FREE(pAdapterInfo);
        pAdapterInfo = (IP_ADAPTER_INFO*)MALLOC(outBufferLength);
        if (pAdapterInfo == NULL) {
            printf("Error allocating memory for IP_ADAPTER_INFO struct\n");
            return;
        }
    }

    if (GetAdaptersInfo(pAdapterInfo, &outBufferLength) == NO_ERROR) {
        while (pAdapterInfo != NULL) {
            printAdpaterInfo(pAdapterInfo);
            pAdapterInfo = pAdapterInfo->Next;
        }
    }

    if (pAdapterInfo)
        FREE(pAdapterInfo);
}