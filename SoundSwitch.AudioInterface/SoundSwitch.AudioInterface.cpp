// SoundSwitch.AudioInterface.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
//#include <stdio.h>
//#include <wchar.h>
//#include <tchar.h>
#include <iostream>
#include "windows.h"
#include "Mmdeviceapi.h"
#include "PolicyConfig.h"
#include "Propidl.h"
#include "Functiondiscoverykeys_devpkey.h"

// This application is by Dave Amenta 
// Source: http://www.daveamenta.com/2011-05/programmatically-or-command-line-change-the-default-sound-playback-device-in-windows-7/


HRESULT SetDefaultAudioPlaybackDevice(LPCWSTR devID)
{	
	IPolicyConfigVista *pPolicyConfig;
	ERole reserved = eConsole;

    HRESULT hr = CoCreateInstance(__uuidof(CPolicyConfigVistaClient), 
		NULL, CLSCTX_ALL, __uuidof(IPolicyConfigVista), (LPVOID *)&pPolicyConfig);
	if (SUCCEEDED(hr))
	{
		hr = pPolicyConfig->SetDefaultEndpoint(devID, reserved);
		pPolicyConfig->Release();
	}
	return hr;
}

HRESULT GetDefaultDeviceID(IMMDeviceEnumerator *pEnum, LPWSTR *ppOutID)
{
        IMMDevice *pDevice = NULL;
        HRESULT hr = pEnum->GetDefaultAudioEndpoint(eRender, eConsole, &pDevice);
                                
        if(FAILED(hr))
        {
                *ppOutID = NULL;
                return hr;
        }

        hr = pDevice->GetId(ppOutID);
        pDevice->Release();

        return hr;
}

int _tmain(int argc, _TCHAR* argv[])
{
		// read the command line option, -1 indicates list devices.
	int option = -1;
	if (argc == 2) option = atoi((char*)argv[1]);
	std::wcout << "SoundSwitch Audio Interface Changer\r\n";
	bool setOutput = false;

	HRESULT hr = CoInitialize(NULL);
	if (SUCCEEDED(hr))
	{
		IMMDeviceEnumerator *pEnum = NULL;
		// Create a multimedia device enumerator.
		hr = CoCreateInstance(__uuidof(MMDeviceEnumerator), NULL,
			CLSCTX_ALL, __uuidof(IMMDeviceEnumerator), (void**)&pEnum);
		if (SUCCEEDED(hr))
		{
            LPWSTR wstrDefaultID = NULL;
                                        
            hr = GetDefaultDeviceID(pEnum, &wstrDefaultID);
            if(FAILED(hr))
            {
                    wstrDefaultID = NULL;
            }

			IMMDeviceCollection *pDevices;

			// Get default device
			IMMDevice *pDevice = NULL;
			hr = pEnum->GetDefaultAudioEndpoint(eRender, eConsole, &pDevice);

			// Enumerate the output devices.
			hr = pEnum->EnumAudioEndpoints(eRender, DEVICE_STATE_ACTIVE | DEVICE_STATE_UNPLUGGED | DEVICE_STATE_DISABLED, &pDevices);
			if (SUCCEEDED(hr))
			{
				UINT count;
				pDevices->GetCount(&count);
				if (SUCCEEDED(hr))
				{
					for (int i = 0; i < count; i++)
					{
						IMMDevice *pDevice;
						hr = pDevices->Item(i, &pDevice);
						if (SUCCEEDED(hr))
						{
							LPWSTR wstrID = NULL;
							hr = pDevice->GetId(&wstrID);
							if (SUCCEEDED(hr))
							{
								IPropertyStore *pStore;
								hr = pDevice->OpenPropertyStore(STGM_READ, &pStore);
								if (SUCCEEDED(hr))
								{
									bool isDefault = false;
                                    if(wstrDefaultID != NULL)
                                    {
                                            LPWSTR wstrID = NULL;
                                            HRESULT hr = pDevice->GetId(&wstrID);

                                            if(!_wcsicmp(wstrID, wstrDefaultID))
                                            {
                                                    isDefault = true;
                                            }

                                            CoTaskMemFree(wstrID);
                                    }

									PROPVARIANT friendlyName;
									PropVariantInit(&friendlyName);
									hr = pStore->GetValue(PKEY_Device_FriendlyName, &friendlyName);
									if (SUCCEEDED(hr))
									{
										// if no options, print the device
										// otherwise, find the selected device and set it to be default
										if (option == -1) {
											//printf("Audio Device %d: %ws\n",i, friendlyName.pwszVal);
											std::wcout << i << ": " << friendlyName.pwszVal;
											if (isDefault == true)
											{
												std::wcout << " (default)";
											}
											std::wcout << "\r\n";
										}
										if (i == option) {
											std::wcout << "Setting output to: " << i << ": " << friendlyName.pwszVal << "\r\n";
											SetDefaultAudioPlaybackDevice(wstrID);
											setOutput = true;
										}
										PropVariantClear(&friendlyName);
									}
									pStore->Release();
								}
							}
							pDevice->Release();
						}
					}
				}
				pDevices->Release();
			}


			//WTF?  If Release() gets called, then redirecting this .exe's output stops working. No idea why.. 
			// pEnum->Release();
		}
	}

	if (option >= 0 && !setOutput) 
	{
		std::cout << "Failed to select device " << option << "\r\n";
		return 1;
	}
	
	return hr;
}

