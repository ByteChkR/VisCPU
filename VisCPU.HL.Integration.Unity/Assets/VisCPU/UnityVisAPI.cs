using System.Collections.Generic;
using UnityEngine;
using VisCPU;

public static class UnityVisApi
{
    private static readonly Dictionary < uint, Object > s_ObjectHandles = new Dictionary < uint, Object >();

    #region Public

    public static uint UAPI_AddPosition( CPU executingCpu )
    {
        int z = ( int ) executingCpu.Pop();
        int y = ( int ) executingCpu.Pop();
        int x = ( int ) executingCpu.Pop();
        uint handle = executingCpu.Pop();

        if ( s_ObjectHandles.ContainsKey( handle ) )
        {
            Object obj = s_ObjectHandles[handle];

            if ( obj is GameObject gobj )
            {
                gobj.transform.position += new Vector3( x, y, z );
            }

            return 1;
        }

        return 0;
    }

    public static uint UAPI_AddPositionX( CPU executingCpu )
    {
        int x = ( int ) executingCpu.Pop();
        uint handle = executingCpu.Pop();

        if ( s_ObjectHandles.ContainsKey( handle ) )
        {
            Object obj = s_ObjectHandles[handle];

            if ( obj is GameObject gobj )
            {
                gobj.transform.position += new Vector3( x, 0, 0 );
            }

            return 1;
        }

        return 0;
    }

    public static uint UAPI_AddPositionY( CPU executingCpu )
    {
        int y = ( int ) executingCpu.Pop();
        uint handle = executingCpu.Pop();

        if ( s_ObjectHandles.ContainsKey( handle ) )
        {
            Object obj = s_ObjectHandles[handle];

            if ( obj is GameObject gobj )
            {
                gobj.transform.position += new Vector3( 0, y, 0 );
            }

            return 1;
        }

        return 0;
    }

    public static uint UAPI_AddPositionZ( CPU executingCpu )
    {
        int z = ( int ) executingCpu.Pop();
        uint handle = executingCpu.Pop();

        if ( s_ObjectHandles.ContainsKey( handle ) )
        {
            Object obj = s_ObjectHandles[handle];

            if ( obj is GameObject gobj )
            {
                gobj.transform.position += new Vector3( 0, 0, z );
            }

            return 1;
        }

        return 0;
    }

    public static uint UAPI_CreateHandle( CPU executingCpu )
    {
        uint len = executingCpu.Pop();
        uint ptr = executingCpu.Pop();
        string name = "";

        for ( uint i = 0; i < len; i++ )
        {
            name += ( char ) executingCpu.MemoryBus.Read( ptr + i );
        }

        GameObject obj = GameObject.Find( name );

        if ( obj )
        {
            uint handle = ( uint ) obj.GetInstanceID();
            s_ObjectHandles[handle] = obj;

            return handle;
        }

        return 0;
    }

    public static uint UAPI_DestroyByHandle( CPU executingCpu )
    {
        uint handle = executingCpu.Pop();

        if ( s_ObjectHandles.ContainsKey( handle ) )
        {
            Object obj = s_ObjectHandles[handle];
            Object.Destroy( obj );
            s_ObjectHandles.Remove( handle );

            return 1;
        }

        return 0;
    }

    public static uint UAPI_DestroyByName( CPU executingCpu )
    {
        uint len = executingCpu.Pop();
        uint ptr = executingCpu.Pop();
        string name = "";

        for ( uint i = 0; i < len; i++ )
        {
            name += ( char ) executingCpu.MemoryBus.Read( ptr + i );
        }

        GameObject obj = GameObject.Find( name );

        if ( obj )
        {
            Object.Destroy( obj );
        }

        return ( uint ) ( obj ? 1 : 0 );
    }

    public static uint UAPI_GetPositionX( CPU executingCpu )
    {
        uint handle = executingCpu.Pop();

        if ( s_ObjectHandles.ContainsKey( handle ) )
        {
            Object obj = s_ObjectHandles[handle];

            if ( obj is GameObject gobj )
            {
                return ( uint ) gobj.transform.position.x;
            }

            return 0;
        }

        return 0;
    }

    public static uint UAPI_GetPositionY( CPU executingCpu )
    {
        uint handle = executingCpu.Pop();

        if ( s_ObjectHandles.ContainsKey( handle ) )
        {
            Object obj = s_ObjectHandles[handle];

            if ( obj is GameObject gobj )
            {
                return ( uint ) gobj.transform.position.y;
            }

            return 0;
        }

        return 0;
    }

    public static uint UAPI_GetPositionZ( CPU executingCpu )
    {
        uint handle = executingCpu.Pop();

        if ( s_ObjectHandles.ContainsKey( handle ) )
        {
            Object obj = s_ObjectHandles[handle];

            if ( obj is GameObject gobj )
            {
                return ( uint ) gobj.transform.position.z;
            }

            return 0;
        }

        return 0;
    }

    public static uint UAPI_Log( CPU executingCpu )
    {
        uint len = executingCpu.Pop();
        uint ptr = executingCpu.Pop();
        string content = "";

        for ( uint i = 0; i < len; i++ )
        {
            content += ( char ) executingCpu.MemoryBus.Read( ptr + i );
        }

        Debug.Log( content );

        return 1;
    }

    public static uint UAPI_LogError( CPU executingCpu )
    {
        uint len = executingCpu.Pop();
        uint ptr = executingCpu.Pop();
        string content = "";

        for ( uint i = 0; i < len; i++ )
        {
            content += ( char ) executingCpu.MemoryBus.Read( ptr + i );
        }

        Debug.LogError( content );

        return 1;
    }

    public static uint UAPI_SetPosition( CPU executingCpu )
    {
        int z = ( int ) executingCpu.Pop();
        int y = ( int ) executingCpu.Pop();
        int x = ( int ) executingCpu.Pop();
        uint handle = executingCpu.Pop();

        if ( s_ObjectHandles.ContainsKey( handle ) )
        {
            Object obj = s_ObjectHandles[handle];

            if ( obj is GameObject gobj )
            {
                gobj.transform.position = new Vector3( x, y, z );
            }

            return 1;
        }

        return 0;
    }

    public static uint UAPI_SetPositionX( CPU executingCpu )
    {
        int x = ( int ) executingCpu.Pop();
        uint handle = executingCpu.Pop();

        if ( s_ObjectHandles.ContainsKey( handle ) )
        {
            Object obj = s_ObjectHandles[handle];

            if ( obj is GameObject gobj )
            {
                gobj.transform.position = new Vector3( x, gobj.transform.position.y, gobj.transform.position.z );
            }

            return 1;
        }

        return 0;
    }

    public static uint UAPI_SetPositionY( CPU executingCpu )
    {
        int y = ( int ) executingCpu.Pop();
        uint handle = executingCpu.Pop();

        if ( s_ObjectHandles.ContainsKey( handle ) )
        {
            Object obj = s_ObjectHandles[handle];

            if ( obj is GameObject gobj )
            {
                gobj.transform.position = new Vector3( gobj.transform.position.x, y, gobj.transform.position.z );
            }

            return 1;
        }

        return 0;
    }

    public static uint UAPI_SetPositionZ( CPU executingCpu )
    {
        int z = ( int ) executingCpu.Pop();
        uint handle = executingCpu.Pop();

        if ( s_ObjectHandles.ContainsKey( handle ) )
        {
            Object obj = s_ObjectHandles[handle];

            if ( obj is GameObject gobj )
            {
                gobj.transform.position = new Vector3( gobj.transform.position.x, gobj.transform.position.y, z );
            }

            return 1;
        }

        return 0;
    }

    #endregion
}
