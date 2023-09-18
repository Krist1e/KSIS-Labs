#include "Task2.h"

BOOL printNetworkResources(LPNETRESOURCE lpnr) {
	DWORD dwResult;
	HANDLE hEnum;
	DWORD cbBuffSize = 16384;

	// enumerate all possible entries
	DWORD cEntries = 0xFFFFFFFF;

	dwResult = WNetOpenEnum(RESOURCE_GLOBALNET, // all network resources
							RESOURCETYPE_ANY,	// all resources
							0,					// enumerate all resources
							lpnr,				// first time is NULL
							&hEnum);			// handle resources

	if (dwResult != NO_ERROR) {
		NetErrorHandler(dwResult, (LPSTR)"WNetOpenEnum");
		return FALSE;
	}

	// pointer to enumerated structures
	LPNETRESOURCE lpLocalResource = (LPNETRESOURCE)MALLOC(cbBuffSize);


	do {

		// initialize buffer
		ZeroMemory(lpLocalResource, cbBuffSize);
		dwResult = WNetEnumResource(hEnum, &cEntries, lpLocalResource, &cbBuffSize);

		if (dwResult == NO_ERROR) {
			for (DWORD i = 0; i < cEntries; i++) {
				DisplayStruct(&lpLocalResource[i]);

				// if container, open recursively
				if (RESOURCEUSAGE_CONTAINER == (lpLocalResource[i].dwUsage & RESOURCEUSAGE_CONTAINER))
					printNetworkResources(&lpLocalResource[i]);
			}
		}
		else if (dwResult != ERROR_NO_MORE_ITEMS) {
			NetErrorHandler(dwResult, (LPSTR)"WNetEnumResource");
			break;
		}
	} while (dwResult != ERROR_NO_MORE_ITEMS);

	FREE(lpLocalResource);

	dwResult = WNetCloseEnum(hEnum);

	if (dwResult != NO_ERROR) {
		NetErrorHandler(dwResult, (LPSTR)"WNetCloseEnum");
		return FALSE;
	}



	return TRUE;
}

void NetErrorHandler(DWORD errorNum, LPSTR funcName) {
	printf("\t[%s] Error: %d \n", funcName, errorNum);

	switch (errorNum) {
	case ERROR_NOT_CONTAINER:
		printf("\tERROR_NOT_CONTAINER\n");
		break;
	case ERROR_INVALID_PARAMETER:
		printf("\tERROR_INVALID_PARAMETER\n");
		break;

	case ERROR_NO_NETWORK:
		printf("\tERROR_NO_NETWORK\n");
		break;
	case ERROR_EXTENDED_ERROR: {
		auto lpError = new DWORD;
		constexpr DWORD nErrorBufferSize = 1024;
		constexpr DWORD nNameBufferSize = 128;
		auto lpErrorBuffer = new wchar_t[nErrorBufferSize];
		auto lpNameBuffer = new wchar_t[nNameBufferSize];

		if (WNetGetLastError(lpError, lpErrorBuffer, nErrorBufferSize, lpNameBuffer, nNameBufferSize) != NO_ERROR)
			printf("\tCouldn't process the error\n");
		else
			printf("\tError details:\n\tName: %ls\n\t%ls\n\n", lpNameBuffer, lpErrorBuffer);
		break;
	}
	default:
		printf("\tUNKNOWN_ERROR\n");
	}
}

void DisplayStruct(LPNETRESOURCE lpnrLocal)
{
	printf("\tScope: ");
	switch (lpnrLocal->dwScope) {
	case (RESOURCE_CONNECTED):
		printf("connected\n");
		break;
	case (RESOURCE_GLOBALNET):
		printf("all resources\n");
		break;
	case (RESOURCE_REMEMBERED):
		printf("remembered\n");
		break;
	default:
		printf("unknown scope %d\n", lpnrLocal->dwScope);
		break;
	}

	printf("\tType: ");
	switch (lpnrLocal->dwType) {
	case (RESOURCETYPE_ANY):
		printf("any\n");
		break;
	case (RESOURCETYPE_DISK):
		printf("disk\n");
		break;
	case (RESOURCETYPE_PRINT):
		printf("print\n");
		break;
	default:
		printf("unknown type %d\n", lpnrLocal->dwType);
		break;
	}

	printf("\tDisplay Type: ");
	switch (lpnrLocal->dwDisplayType) {
	case (RESOURCEDISPLAYTYPE_GENERIC):
		printf("generic\n");
		break;
	case (RESOURCEDISPLAYTYPE_DOMAIN):
		printf("domain\n");
		break;
	case (RESOURCEDISPLAYTYPE_SERVER):
		printf("server\n");
		break;
	case (RESOURCEDISPLAYTYPE_SHARE):
		printf("share\n");
		break;
	case (RESOURCEDISPLAYTYPE_FILE):
		printf("file\n");
		break;
	case (RESOURCEDISPLAYTYPE_GROUP):
		printf("group\n");
		break;
	case (RESOURCEDISPLAYTYPE_NETWORK):
		printf("network\n");
		break;
	default:
		printf("unknown display type %d\n", lpnrLocal->dwDisplayType);
		break;
	}

	printf("\tUsage: 0x%x = ", lpnrLocal->dwUsage);
	if (lpnrLocal->dwUsage & RESOURCEUSAGE_CONNECTABLE)
		printf("connectable ");
	if (lpnrLocal->dwUsage & RESOURCEUSAGE_CONTAINER)
		printf("container ");
	printf("\n");

	printf("\tLocal Name: %S\n", lpnrLocal->lpLocalName);
	printf("\tRemote Name: %S\n", lpnrLocal->lpRemoteName);
	printf("\tComment: %S\n", lpnrLocal->lpComment);
	printf("\tProvider: %S\n", lpnrLocal->lpProvider);
	printf("\n");
}