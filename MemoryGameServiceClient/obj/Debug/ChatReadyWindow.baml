   M S B A M L   `   `   `  ����  MemoryGameServiceClient-    'MemoryGameServiceClient.ChatReadyWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  ChatReadyWindow��5   	   $��477.78��6!   $��679.34��61    � 
ResizeMode$ NoResize=�6@   ��5      �� 5   
    �� EndPoint$ 0.5,1&�6    �� 
StartPoint$ 0.5,0&�6/   .��5      �  � Color$ Black��6    � Offset$ 0.975Q�6)   � 5      $ 	#FFB88585��6   $ 0.051Q�6-   5	      .��5      � .+�5   
   �� -   5         ��Name$ chatMsg_txtBox��5      $
��Left=�6*   $��33��6E   $��
10,405,0,0q�6Q    �� TextWrapping$
 Wrap=�6e   $	��Top=�6y   $	��354��6�   $��14(�6�   6�   �� 5   
   -   5         ��Name$ 
sendMsgBtn��5      $
��Send��6%   $
��Left=�64   $��369,405,0,0q�6O   $	��Top=�6d   $��75��6|   $��33��6�   $
��Bold"�6�   $��14(�6�   ���	#FF1D95286�   �� 5   
   -   5        $ 	Ready_btn��5      $��Ready��6$   $
��Left=�64   $��461,405,0,0q�6O   $	��Top=�6d   $	��202��6|   $��33��6�   $��18(�6�   $
��Bold"�6�   �� 5   
   -   5        	 ��Name$	 userNameState_listView��5      $
��Left=�63   $	��390��6N   $��	461,6,0,0q�6[   $	��Top=�6n   $	��202��6�   
 �� View
 5      �� 5      .P�5      �� $��	User Name��6%    �� Width$	 100��6d    �� DisplayMemberBinding 68   �� *
playername+6d   �� 5      $��State��6%   $	 100��6^    64   �� *
is_ready+6^   5      5      �� 5   
   -   5        $	 chatBox_listView��5      $
��Left=�6-   $	��390��6H   $��10,6,0,0q�6U   $	��Top=�6g   $	��434��6   
 5      �� 5      .P�5      �� $��	User Name��6%   $	 100��6d    68   �� *
senderName+6d   �� 5      $��message��6%   $	 329��6b    66   �� *
MsgContent+6b   5      5      5       5!      