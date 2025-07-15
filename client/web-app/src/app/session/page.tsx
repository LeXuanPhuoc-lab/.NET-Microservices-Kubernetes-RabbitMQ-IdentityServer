import React from 'react'
import { auth } from '@/app/auth'
import Heading from '../components/Heading'

export default async function Session() {
    const session = await auth();

    return (
        <div>
            <Heading title='Session dashboard' subTitle='' />

            <div className='bg-blue-200 border-2 border-blue-500'>
                <h3 className='text-lg'>Session Data</h3>
                <pre className='whitespace-pre-wrap break-all'>{JSON.stringify(session, null, 2)}</pre>
            </div>
        </div>
    )
}