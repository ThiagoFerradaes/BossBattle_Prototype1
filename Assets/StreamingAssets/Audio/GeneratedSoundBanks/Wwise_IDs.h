/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID PLAY_BOAT = 1391074924U;
        static const AkUniqueID PLAY_CLOSE_DOOR = 3898429045U;
        static const AkUniqueID PLAY_DASH_BASTIAN = 511398661U;
        static const AkUniqueID PLAY_DOWNSTAIRS_TRANSITION = 4197013950U;
        static const AkUniqueID PLAY_FOOTSTEPSSWITCH = 2975023323U;
        static const AkUniqueID PLAY_FURNITURE_OFF = 708373978U;
        static const AkUniqueID PLAY_FURNITURE_ON = 2049827036U;
        static const AkUniqueID PLAY_KRAKEN_STAGE = 3978310627U;
        static const AkUniqueID PLAY_MAIN_THEME = 3568813383U;
        static const AkUniqueID PLAY_MOPPING = 1421007458U;
        static const AkUniqueID PLAY_OPEN_DOOR = 2427215529U;
        static const AkUniqueID PLAY_TAVERN_MUSIC = 2634924088U;
        static const AkUniqueID PLAY_WATER = 441572235U;
        static const AkUniqueID PLAY_WIND = 1020223172U;
    } // namespace EVENTS

    namespace SWITCHES
    {
        namespace FOOTSTEP_SIDE
        {
            static const AkUniqueID GROUP = 35756383U;

            namespace SWITCH
            {
                static const AkUniqueID LEFT = 4109362U;
                static const AkUniqueID RIGHT = 3893817417U;
            } // namespace SWITCH
        } // namespace FOOTSTEP_SIDE

        namespace FOOTSTEP_SURFACE
        {
            static const AkUniqueID GROUP = 1833605183U;

            namespace SWITCH
            {
                static const AkUniqueID SAND = 803837735U;
                static const AkUniqueID STONE = 1216965916U;
                static const AkUniqueID WOOD = 2058049674U;
            } // namespace SWITCH
        } // namespace FOOTSTEP_SURFACE

        namespace FURNITURE_WEIGHT
        {
            static const AkUniqueID GROUP = 3102223980U;

            namespace SWITCH
            {
                static const AkUniqueID HEAVY = 2732489590U;
                static const AkUniqueID LIGHT = 1935470627U;
                static const AkUniqueID MEDIUM = 2849147824U;
            } // namespace SWITCH
        } // namespace FURNITURE_WEIGHT

        namespace KRAKEN_STAGE
        {
            static const AkUniqueID GROUP = 4042782448U;

            namespace SWITCH
            {
                static const AkUniqueID LEVEL_01 = 987635873U;
                static const AkUniqueID LEVEL_02 = 987635874U;
                static const AkUniqueID LEVEL_03 = 987635875U;
            } // namespace SWITCH
        } // namespace KRAKEN_STAGE

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID AMBIENTVOLUME = 3546521921U;
        static const AkUniqueID GENERALVOLUME = 421429125U;
        static const AkUniqueID MUSICVOLUME = 2346531308U;
        static const AkUniqueID SFXVOLUME = 988953028U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID MASTER_SOUNDBANK = 2469504869U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID AMBIENT_BUS = 3148666284U;
        static const AkUniqueID MAIN_AUDIO_BUS = 2246998526U;
        static const AkUniqueID MUSIC_BUS = 2680856269U;
        static const AkUniqueID SFX_BUS = 213475909U;
    } // namespace BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
