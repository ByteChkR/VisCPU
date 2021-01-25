using System.Collections.Generic;
using UnityEngine;
using System;
using VisCPU;

public static class UnityVisApi
{
    private static readonly Dictionary < uint, Object > s_ObjectHandles = new Dictionary < uint, Object >();

    #region Public

    public static uint UAPI_AddPosition( Cpu executingCpu )
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

    public static uint UAPI_AddPositionX(Cpu executingCpu )
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

    public static uint UAPI_AddPositionY(Cpu executingCpu )
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

    public static uint UAPI_AddPositionZ(Cpu executingCpu )
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

    public static uint UAPI_CreateHandle(Cpu executingCpu )
    {
        uint len = executingCpu.Pop();
        uint ptr = executingCpu.Pop();
        StringBuilder sb = new StringBuilder();

        for ( uint i = 0; i < len; i++ )
        {
            sb.Append( ( char ) executingCpu.MemoryBus.Read( ptr + i ));
        }

        GameObject obj = GameObject.Find( sb.ToString() );

        if ( obj )
        {
            uint handle = ( uint ) obj.GetInstanceID();
            s_ObjectHandles[handle] = obj;

            return handle;
        }

        return 0;
    }

    public static uint UAPI_DestroyByHandle(Cpu executingCpu )
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

    public static uint UAPI_DestroyByName(Cpu executingCpu )
    {
        uint len = executingCpu.Pop();
        uint ptr = executingCpu.Pop();
        StringBuilder sb = new StringBuilder();

        for ( uint i = 0; i < len; i++ )
        {
            sb.Append(( char ) executingCpu.MemoryBus.Read( ptr + i ));
        }

        GameObject obj = GameObject.Find( sb.ToString() );

        if ( obj )
        {
            Object.Destroy( obj );
        }

        return ( uint ) ( obj ? 1 : 0 );
    }

    public static uint UAPI_GetPositionX(Cpu executingCpu )
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

    public static uint UAPI_GetPositionY(Cpu executingCpu )
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

    public static uint UAPI_GetPositionZ(Cpu executingCpu )
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

    public static uint UAPI_Log(Cpu executingCpu )
    {
        uint len = executingCpu.Pop();
        uint ptr = executingCpu.Pop();
        StringBuilder sb = new StringBuilder();

        for ( uint i = 0; i < len; i++ )
        {

            sb.Append(( char ) executingCpu.MemoryBus.Read( ptr + i ));
        }

        Debug.Log( sb.ToString() );

        return 1;
    }

    public static uint UAPI_LogError(Cpu executingCpu )
    {
        uint len = executingCpu.Pop();
        uint ptr = executingCpu.Pop();
        StringBuilder sb = new StringBuilder();

        for ( uint i = 0; i < len; i++ )
        {
            sb.Append( ( char ) executingCpu.MemoryBus.Read( ptr + i ));
        }

        Debug.LogError( sb.ToString() );

        return 1;
    }

    public static uint UAPI_SetPosition(Cpu executingCpu )
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

    public static uint UAPI_SetPositionX(Cpu executingCpu )
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

    public static uint UAPI_SetPositionY(Cpu executingCpu )
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

    public static uint UAPI_SetPositionZ(Cpu executingCpu )
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
