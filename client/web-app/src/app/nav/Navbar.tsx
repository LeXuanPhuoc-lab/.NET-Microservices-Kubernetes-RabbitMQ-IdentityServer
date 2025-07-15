'use client'

import React from 'react'
import Search from './Search'
import Logo from './Logo'
import LoginButton from './LoginButton'
import UserActions from './UserActions'
import { useSession } from 'next-auth/react'

export default function Navbar() {
  const session = useSession();
  const user = session.data?.user;

  return (
    <header className='
      sticky top-0 z-50 flex justify-between bg-white p-5 items-center text-gray-800 shadow-md
    '>
      <Logo />
      <Search />
      {user ? (
        <UserActions user={user} />
      ) : (
        <LoginButton />)
      }
    </header>
  )
}
