'use client'

import React from 'react'
import { useParamsStore } from '../hooks/useParamsStore'
import { Button } from 'flowbite-react'
import Heading from './Heading'
import { signIn } from 'next-auth/react';

type Props = {
    title?: string
    subTitle?: string
    showReset?: boolean
    showLogin?: boolean
    callBackUrl?: string
}

const EmptyFilter = ({
    title = 'No matches for this filter',
    subTitle = 'Try changing or resetting the filter',
    showReset,
    showLogin,
    callBackUrl
}: Props) => {
    const resetParams = useParamsStore(state => state.resetParams);

    return (
        <div className='h-[40vh] flex flex-col gap-2 justify-center items-center shadow-lg'>
            <Heading title={title} subTitle={subTitle} center />
            <div className='mt-4'>
                {showReset && (
                    <Button outline onClick={resetParams}>Remove Filters</Button>
                )}
                {showLogin && (
                    <Button outline onClick={() => signIn('id-server', { redirectTo: callBackUrl })}>
                        Login
                    </Button>
                )}
            </div>
        </div>
    )
}

export default EmptyFilter