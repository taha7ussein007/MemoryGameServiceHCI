   M S B A M L   `   `   `  ����  MemoryGameServiceClient(    "MemoryGameServiceClient.JoinWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$
  Join��5   	   $��489.301��6   $��779.188��6'    � 
ResizeMode$ NoResize=�67   ��5      �� 5   
    �� EndPoint$ 0.5,1&�6    �� 
StartPoint$ 0.5,0&�6/   .��5      �  � Color$ Black��6    � Offset$ 1Q�6)   � 5      $ White��6   $ 0.068Q�6)   5	      .��5      � $��42,0,0,0q�6   .+�5   
   �� $��Enter Room ID��6   $
��Left=�6(   $��503,336,0,0q�6C   $	��Top=�6X   $
��Bold"�6p   $��28(�6�   $	��206��6�   ���	#FFEC5E5E$��54��6�    J� Effect 5      3  -System.Windows.Media.Effects.DropShadowEffect  5      5      �� 5   
   -   5         ��Name$ 
RoomId_txt��5      $��42��6&   $��503,390,0,0q�62    �� TextWrapping$
 Wrap=�6G   $	��Top=�6[   $	��206��6s   $��24(�6   $
��Left=�6�   .��6�    5        5      .��5      5      �� 5   
   -   5        	 ��Name$	 Connect_btn��5      $��Connect��6&   $
��Left=�68   $��-26,350,0,0q�6S   $	��Top=�6h   $	��511��6�   $��86��6�   $��48(�6�   $
��Bold"�6�    5        5      5      �� 5   
   -   5        
 ��Name$
 	listRooms��5      $
��Left=�6&   $��-26,10,0,130q�6A   $	��747��6W    �� SelectionMode$ Single=�6c    �� View 5      �� 5      .P�5      �� $��ID��6%    �� Width$	 128��6U    �� DisplayMemberBinding 61   �� *ID+6U   �� 5      $��	Room Name��6%   $	 150��6b    68   �� *
RoomName+6b   �� 5       $��Host UserName��6%   $	 180��6j    6<   �� *HostUserName+6j   �� 5!      $��Number Of Players��6%   $	 115��6p    6@   �� *NumberOfPlayer+6p   �� 5"      $��State��6%   $ 80��6[    64   �� *State+6[   �� 5#      $��Level��6%   $ 40��6[    64   �� *Level+6[   �� 5$      $��Limit��6%   $ 50��6`    64   �� *
maxPlayers+6`   5%      5'      5)      5*      